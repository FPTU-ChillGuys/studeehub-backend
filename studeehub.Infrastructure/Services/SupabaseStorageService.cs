using Microsoft.Extensions.Configuration;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using System.Text;
using System.Text.Json;

namespace studeehub.Infrastructure.Services
{
	public class SupabaseStorageService : ISupabaseStorageService
	{
		private readonly Supabase.Client _client;
		private readonly IConfiguration _config;
		private readonly SemaphoreSlim _initLock = new(1, 1);
		private volatile bool _initialized;

		public SupabaseStorageService(IConfiguration config)
		{
			_config = config;
			_client = new Supabase.Client(
				_config["Supabase:Url"] ?? throw new Exception("Supabase url not found!"),
				_config["Supabase:Key"]
			);
		}

		// Ensure InitializeAsync is only called once concurrently and is safe for many parallel uploads.
		private async Task EnsureInitializedAsync()
		{
			if (_initialized) return;

			await _initLock.WaitAsync();
			try
			{
				if (!_initialized)
				{
					await _client.InitializeAsync();
					_initialized = true;
				}
			}
			finally
			{
				_initLock.Release();
			}
		}

		/// <summary>
		/// Uploads a user avatar to Supabase storage and returns a signed URL.
		/// Supports common image extensions (png, jpg, jpeg, gif, webp, bmp, svg).
		/// If extension cannot be determined it falls back to .png.
		/// </summary>
		public async Task<string?> UploadUserAvatarAsync(Stream fileStream, string userId, string? fileName = null, string bucket = "studeehub_bucket")
		{
			if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));
			if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("userId is required", nameof(userId));

			await EnsureInitializedAsync();

			// Read stream into byte array (we need bytes to possibly detect type)
			byte[] fileBytes;
			using (var ms = new MemoryStream())
			{
				await fileStream.CopyToAsync(ms);
				fileBytes = ms.ToArray();
			}

			// Try extension from fileName first
			string? extension = null;
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				extension = NormalizeAndValidateExtension(Path.GetExtension(fileName));
			}

			// If not available or not allowed, try to detect from bytes
			if (string.IsNullOrWhiteSpace(extension))
			{
				extension = DetectImageExtensionFromBytes(fileBytes);
			}

			// Final fallback
			extension ??= ".png";

			// Build a predictable avatar path: avatars/{userId}/{guid}{ext}
			var uniqueName = $"avatars/{userId}/{Guid.NewGuid():N}{extension}";

			var storage = _client.Storage;
			var bucketRef = storage.From(bucket);

			var uploadResponse = await bucketRef.Upload(fileBytes, uniqueName);

			if (uploadResponse == null)
				return null;

			try
			{
				var signedUrl = await bucketRef.CreateSignedUrl(uniqueName, 3600);
				return string.IsNullOrWhiteSpace(signedUrl) ? null : signedUrl;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Ensure profileUrl contains a valid (non-expired) signed token.
		/// If token missing or expired a fresh signed url is created and returned.
		/// Returns null if unable to create a signed URL.
		/// </summary>
		public async Task<string?> EnsureSignedUrlAsync(string? profileUrl, string userId, int expiresInSeconds = 3600, string bucket = "studeehub_bucket")
		{
			if (string.IsNullOrWhiteSpace(profileUrl))
				return null;

			// Try to parse token from the URL query string
			string? token = null;
			try
			{
				var uri = new Uri(profileUrl);
				var query = uri.Query; // starts with '?'
				if (!string.IsNullOrEmpty(query))
				{
					var qs = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
					foreach (var kv in qs)
					{
						var parts = kv.Split('=', 2);
						if (parts.Length == 2 && parts[0].Equals("token", StringComparison.OrdinalIgnoreCase))
						{
							token = Uri.UnescapeDataString(parts[1]);
							break;
						}
					}
				}
			}
			catch
			{
				// If the URL is malformed we still attempt to regenerate from stored path below
			}

			// If token exists and not expired, return original URL
			if (!string.IsNullOrWhiteSpace(token))
			{
				var exp = TryGetJwtExpiry(token);
				if (exp.HasValue && exp.Value > DateTimeOffset.UtcNow.AddSeconds(30)) // small buffer
				{
					return profileUrl;
				}
			}

			// Extract the object path inside the bucket from the URL and create a new signed URL
			var objectPath = ExtractFilePathFromUrl(profileUrl);
			if (string.IsNullOrWhiteSpace(objectPath))
				return null;

			await EnsureInitializedAsync();

			try
			{
				var bucketRef = _client.Storage.From(bucket);
				var signedUrl = await bucketRef.CreateSignedUrl(objectPath, expiresInSeconds);
				return string.IsNullOrWhiteSpace(signedUrl) ? null : signedUrl;
			}
			catch
			{
				return null;
			}
		}

		// Extracts the path inside the bucket from a Supabase signed/unsigned URL.
		// Example input: https://<project>.supabase.co/storage/v1/object/sign/<bucket>/<path/to/file>?token=...
		// Returns "path/to/file"
		private static string? ExtractFilePathFromUrl(string? fileUrl)
		{
			if (string.IsNullOrWhiteSpace(fileUrl))
				return null;

			try
			{
				var uri = new Uri(fileUrl);
				var path = uri.AbsolutePath; // /storage/v1/object/sign/<bucket>/<file-path>

				var marker = "/storage/v1/object/sign/";
				var idx = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
				if (idx < 0)
					return null;

				var relative = path.Substring(idx + marker.Length); // <bucket>/<file-path>
				var firstSlash = relative.IndexOf('/');
				if (firstSlash < 0)
					return null;

				return relative.Substring(firstSlash + 1);
			}
			catch
			{
				return null;
			}
		}

		// Try to get the 'exp' claim (UNIX seconds) from a JWT token without validating signature.
		private static DateTimeOffset? TryGetJwtExpiry(string token)
		{
			if (string.IsNullOrWhiteSpace(token))
				return null;

			try
			{
				var parts = token.Split('.');
				if (parts.Length < 2) return null;

				string payload = parts[1];
				// base64url -> normal base64
				payload = payload.Replace('-', '+').Replace('_', '/');
				switch (payload.Length % 4)
				{
					case 2: payload += "=="; break;
					case 3: payload += "="; break;
				}

				var bytes = Convert.FromBase64String(payload);
				var json = Encoding.UTF8.GetString(bytes);
				using var doc = JsonDocument.Parse(json);
				if (doc.RootElement.TryGetProperty("exp", out var expProp))
				{
					if (expProp.ValueKind == JsonValueKind.Number && expProp.TryGetInt64(out long seconds))
					{
						return DateTimeOffset.FromUnixTimeSeconds(seconds);
					}
				}
			}
			catch
			{
				// ignore parse errors
			}

			return null;
		}

		private static string? NormalizeAndValidateExtension(string? ext)
		{
			if (string.IsNullOrWhiteSpace(ext))
				return null;

			ext = ext.Trim().ToLowerInvariant();
			if (!ext.StartsWith('.')) ext = "." + ext;

			// Allowed image extensions
			var allowed = new[] { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".svg" };
			if (allowed.Contains(ext))
			{
				// Prefer .jpg instead of .jpeg for consistency
				if (ext == ".jpeg") return ".jpg";
				return ext;
			}

			return null;
		}

		private static string? DetectImageExtensionFromBytes(byte[] bytes)
		{
			if (bytes == null || bytes.Length < 4)
				return null;

			// JPEG (FF D8 FF)
			if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
				return ".jpg";

			// PNG (89 50 4E 47 0D 0A 1A 0A)
			if (bytes.Length >= 8 &&
				bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
				return ".png";

			// GIF (47 49 46 38)
			if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x38)
				return ".gif";

			// BMP (42 4D)
			if (bytes[0] == 0x42 && bytes[1] == 0x4D)
				return ".bmp";

			// WEBP: "RIFF"...."WEBP"
			if (bytes.Length >= 12 &&
				bytes[0] == 'R' && bytes[1] == 'I' && bytes[2] == 'F' && bytes[3] == 'F' &&
				bytes[8] == 'W' && bytes[9] == 'E' && bytes[10] == 'B' && bytes[11] == 'P')
				return ".webp";

			// SVG: textual, may start with '<?xml' or '<svg'
			try
			{
				var header = Encoding.UTF8.GetString(bytes.Take(Math.Min(bytes.Length, 512)).ToArray()).TrimStart();
				if (header.StartsWith("<svg", StringComparison.OrdinalIgnoreCase) ||
					(header.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) && header.IndexOf("<svg", StringComparison.OrdinalIgnoreCase) >= 0))
				{
					return ".svg";
				}
			}
			catch
			{
				// ignore decoding errors
			}

			return null;
		}
	}
}

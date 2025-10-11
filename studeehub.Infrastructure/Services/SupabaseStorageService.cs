using Microsoft.Extensions.Configuration;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

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

		// Synchronous extraction returned as a Task to match the interface signature.
		public Task<string> ExtractFilePathFromUrl(string fileUrl)
		{
			if (string.IsNullOrWhiteSpace(fileUrl))
				throw new ArgumentException("fileUrl is null or empty", nameof(fileUrl));

			// Use Uri to safely parse
			var uri = new Uri(fileUrl);
			var path = uri.AbsolutePath;

			// Supabase URLs typically have this pattern:
			// /storage/v1/object/sign/<bucket>/<file-path>
			var marker = "/storage/v1/object/sign/";
			var idx = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
			if (idx < 0)
				throw new ArgumentException("Invalid Supabase file URL format", nameof(fileUrl));

			// Extract the part after the marker
			var relativePath = path.Substring(idx + marker.Length);

			// Optional: if you only want the path *inside* the bucket (remove bucket name)
			var firstSlash = relativePath.IndexOf('/');
			if (firstSlash > -1)
				relativePath = relativePath.Substring(firstSlash + 1);

			return Task.FromResult(relativePath);
		}

		public async Task<bool> DeleteFileAsync(string filePath, string bucket = "studeehub_bucket")
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("filePath is null or empty", nameof(filePath));

			await EnsureInitializedAsync();

			var storage = _client.Storage;
			var bucketRef = storage.From(bucket);

			var response = await bucketRef.Remove(new List<string> { filePath });

			// If SDK returns a list of removed files, ensure there is at least one removed.
			if (response == null)
				return false;

			// Try to determine success in a safe way
			try
			{
				// If response is a list-like object
				if (response is IEnumerable<string> list)
					return list.Any();

				// Fallback: non-null response indicates success for some SDK versions
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string bucket = "studeehub_bucket")
		{
			if (fileStream == null)
				throw new ArgumentNullException(nameof(fileStream));
			if (string.IsNullOrWhiteSpace(fileName))
				throw new ArgumentException("fileName is null or empty", nameof(fileName));

			await EnsureInitializedAsync();

			var storage = _client.Storage;
			var bucketRef = storage.From(bucket);

			// Read stream into byte array
			byte[] fileBytes;
			using (var ms = new MemoryStream())
			{
				await fileStream.CopyToAsync(ms);
				fileBytes = ms.ToArray();
			}

			// Extract file extension
			var extension = Path.GetExtension(fileName);
			var baseName = Path.GetFileNameWithoutExtension(fileName);

			// Generate unique filename to avoid duplicates
			var uniqueName = $"{baseName}_{Guid.NewGuid():N}{extension}";

			var response = await bucketRef.Upload(fileBytes, uniqueName);

			// SDKs differ on return types. Treat non-null/non-empty as success and return signed URL.
			if (response != null)
			{
				try
				{
					var signedUrl = await GenerateSignedUrlAsync(uniqueName, 3600, bucket);
					return signedUrl;
				}
				catch
				{
					// If CreateSignedUrl throws for some SDK changes, return null to indicate failure.
					return null;
				}
			}

			return null;
		}

		// Optional: a convenience batch upload that reuses UploadFileAsync internally.
		// Not required by the interface but can be used by concrete callers if desired.
		public async Task<IList<string?>> UploadFilesAsync(IEnumerable<(Stream FileStream, string FileName)> files, string bucket = "studeehub_bucket")
		{
			if (files == null)
				throw new ArgumentNullException(nameof(files));

			var results = new List<string?>();

			// Sequential to avoid overwhelming SDK / concurrency issues; callers can run parallel if needed.
			foreach (var (stream, name) in files)
			{
				results.Add(await UploadFileAsync(stream, name, bucket));
			}

			return results;
		}

		public async Task<string> GenerateSignedUrlAsync(string filePath, int expiresInSeconds = 3600, string bucket = "studeehub_bucket")
		{
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("filePath is null or empty", nameof(filePath));

			if (expiresInSeconds <= 0)
				throw new ArgumentOutOfRangeException(nameof(expiresInSeconds), "expiresInSeconds must be greater than zero.");

			await EnsureInitializedAsync();

			var storage = _client.Storage;
			var bucketRef = storage.From(bucket);

			try
			{
				var signedUrl = await bucketRef.CreateSignedUrl(filePath, expiresInSeconds);

				if (string.IsNullOrWhiteSpace(signedUrl))
					throw new InvalidOperationException("Supabase SDK returned an empty signed URL.");

				return signedUrl;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Failed to create signed URL for '{filePath}' in bucket '{bucket}'.", ex);
			}
		}
	}
}

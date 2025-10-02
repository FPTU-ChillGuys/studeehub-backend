using Microsoft.Extensions.Configuration;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;

namespace studeehub.Infrastructure.Services
{
	public class SupabaseStorageService : ISupabaseStorageService
	{
		private readonly Supabase.Client _client;
		private readonly IConfiguration _config;

		public SupabaseStorageService(IConfiguration config)
		{
			_config = config;
			_client = new Supabase.Client(
				_config["Supabase:Url"] ?? throw new Exception("Supabase url not found!"),
				_config["Supabase:Key"]
			);
		}

		public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string bucket = "Studeehub_Bucket")
		{
			await _client.InitializeAsync();

			var storage = _client.Storage;
			var bucketRef = storage.From(bucket);

			// Read stream into byte array
			byte[] fileBytes;
			using (var ms = new MemoryStream())
			{
				await fileStream.CopyToAsync(ms);
				fileBytes = ms.ToArray();
			}

			var response = await bucketRef.Upload(fileBytes, fileName);

			if (!string.IsNullOrEmpty(response))
			{
				// Return public URL
				return bucketRef.GetPublicUrl(fileName);
			}

			return null;
		}
	}
}

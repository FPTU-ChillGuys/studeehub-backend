namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface ISupabaseStorageService
	{
		public Task<string> ExtractFilePathFromUrl(string fileUrl);
		public Task<string?> UploadFileAsync(Stream fileStream, string fileName, string bucket = "Studeehub_Bucket");
		public Task<bool> DeleteFileAsync(string filePath, string bucket = "Studeehub_Bucket");
		public Task<string> GenerateSignedUrlAsync(string filePath, int expiresInSeconds = 3600, string bucket = "Studeehub_Bucket");

	}
}

namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface ISupabaseStorageService
	{
		public Task<string?> UploadUserAvatarAsync(Stream fileStream, string userId, string? fileName = null, string bucket = "studeehub_bucket");

		public Task<string?> EnsureSignedUrlAsync(string? profileUrl, string userId, int expiresInSeconds = 3600, string bucket = "studeehub_bucket");
	}
}

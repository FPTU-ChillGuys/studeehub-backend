namespace studeehub.Application.Interfaces.Services.ThirdPartyServices
{
	public interface ISupabaseStorageService
	{
		Task<string?> UploadFileAsync(Stream fileStream, string fileName, string bucket = "Studeehub_Bucket");
	}
}

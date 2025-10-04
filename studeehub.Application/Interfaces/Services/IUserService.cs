namespace studeehub.Application.Interfaces.Services
{
	public interface IUserService
	{
		public Task<bool> IsUserExistAsync(Guid userId);
	}
}

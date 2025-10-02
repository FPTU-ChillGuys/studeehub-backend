using Google.Apis.Auth;
using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Repositories
{
	public interface IAuthRepository
	{
		public Task<string> GenerateJwtToken(User user, string role);
		public Task<User> RegisterViaGoogleAsync(GoogleJsonWebSignature.Payload payload);

		public Task ConfirmEmailAsync(User user);
		public Task<string> GenerateAndSaveRefreshToken(User user);
		public Task<User> ValidateRefreshToken(Guid userId, string refreshToken);
	}
}

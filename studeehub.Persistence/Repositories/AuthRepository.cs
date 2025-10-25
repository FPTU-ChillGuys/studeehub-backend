using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace studeehub.Persistence.Repositories
{
	public class AuthRepository : IAuthRepository
	{
		private readonly UserManager<User> _userManager;
		private readonly StudeeHubDBContext _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthRepository> _logger;
		private readonly string _secretKey;
		private readonly string _issuer;
		private readonly string _audience;
		private readonly int _expiryMinutes;

		public AuthRepository(
			StudeeHubDBContext context,
			IConfiguration configuration,
			UserManager<User> userManager,
			ILogger<AuthRepository> logger)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			_secretKey = _configuration["Authentication:Key"]
				?? throw new ArgumentNullException("Authentication:Key not found in configuration");
			_issuer = _configuration["Authentication:Issuer"]
				?? throw new ArgumentNullException("Authentication:Issuer not found in configuration");
			_audience = _configuration["Authentication:Audience"]
				?? throw new ArgumentNullException("Authentication:Audience not found in configuration");

			_expiryMinutes = int.TryParse(_configuration["Authentication:ExpiryInMinutes"], out var minutes)
				? minutes
				: throw new ArgumentNullException("Authentication:ExpiryInMinutes not found or invalid");
		}

		public async Task ConfirmEmailAsync(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			user.EmailConfirmed = true;
			await _userManager.UpdateAsync(user);
		}

		public async Task<string> GenerateAndSaveRefreshToken(User user)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));

			var refreshToken = GenerateRefreshToken();
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

			if (!_context.ChangeTracker.Entries<User>().Any(e => e.Entity == user))
			{
				_context.Users.Update(user);
			}

			await _context.SaveChangesAsync();
			return refreshToken;
		}

		public Task<string> GenerateJwtToken(User user, string role)
		{
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (string.IsNullOrWhiteSpace(role)) throw new ArgumentNullException(nameof(role));

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim("id", user.Id.ToString()),
				new Claim("email", user.Email ?? string.Empty),
				new Claim("role", role),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.UtcNow.AddMinutes(_expiryMinutes);

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: expires,
				signingCredentials: creds
			);

			return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
		}

		public async Task<User> RegisterViaGoogleAsync(GoogleJsonWebSignature.Payload payload)
		{
			if (payload == null || string.IsNullOrWhiteSpace(payload.Email))
			{
				_logger.LogWarning("RegisterViaGoogleAsync called with null/invalid payload.");
				return null!;
			}

			try
			{
				var existing = await _userManager.FindByEmailAsync(payload.Email);
				if (existing != null)
				{
					_logger.LogInformation("User already exists for email {Email}. Returning existing user.", payload.Email);
					return existing;
				}

				var newUser = new User
				{
					Email = payload.Email,
					UserName = payload.Email,
					FullName = payload.Name ?? string.Empty,
					EmailConfirmed = true,
					IsActive = true,
					Address = payload.Locale ?? string.Empty,
					ProfilePictureUrl = string.IsNullOrWhiteSpace(payload.Picture) ? string.Empty : payload.Picture.Trim()
				};

				var tempPassword = GenerateTemporaryPassword(12);
				var createResult = await _userManager.CreateAsync(newUser, tempPassword);
				if (!createResult.Succeeded)
				{
					_logger.LogWarning(
						"Failed to create user via Google for {Email}. Errors: {Errors}",
						payload.Email,
						string.Join(" | ", createResult.Errors.Select(e => e.Description)));

					return null!;
				}

				const string defaultRole = "User";
				var roleExists = await _context.Roles.AnyAsync(r => r.Name == defaultRole);
				if (!roleExists)
				{
					_logger.LogWarning("Default role '{Role}' does not exist. Skipping role assignment for user {Email}.", defaultRole, payload.Email);
				}
				else
				{
					var roleResult = await _userManager.AddToRoleAsync(newUser, defaultRole);
					if (!roleResult.Succeeded)
					{
						_logger.LogWarning(
							"Failed to add role '{Role}' to user {Email}. Errors: {Errors}",
							defaultRole,
							payload.Email,
							string.Join(" | ", roleResult.Errors.Select(e => e.Description)));
					}
				}

				return newUser;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error in RegisterViaGoogleAsync for email {Email}", payload.Email);
				return null!;
			}
		}

		private string GenerateTemporaryPassword(int length = 12)
		{
			if (length < 8) length = 8;

			const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			const string lower = "abcdefghijklmnopqrstuvwxyz";
			const string digits = "0123456789";
			const string special = "!@#$%^&*()-_=+[]{};:,.<>?";

			var all = upper + lower + digits + special;
			var chars = new List<char>
			{
				upper[RandomNumberGenerator.GetInt32(upper.Length)],
				lower[RandomNumberGenerator.GetInt32(lower.Length)],
				digits[RandomNumberGenerator.GetInt32(digits.Length)],
				special[RandomNumberGenerator.GetInt32(special.Length)]
			};

			for (int i = chars.Count; i < length; i++)
			{
				chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);
			}

			var arr = chars.ToArray();
			for (int i = arr.Length - 1; i > 0; i--)
			{
				int j = RandomNumberGenerator.GetInt32(i + 1);
				var tmp = arr[i];
				arr[i] = arr[j];
				arr[j] = tmp;
			}

			return new string(arr);
		}

		public async Task<User> ValidateRefreshToken(Guid userId, string refreshToken)
		{
			if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
			if (string.IsNullOrWhiteSpace(refreshToken)) throw new ArgumentNullException(nameof(refreshToken));

			var user = await _context.Users
				.SingleOrDefaultAsync(u => u.Id == userId && u.RefreshToken == refreshToken);

			if (user == null) return null!;
			if (user.RefreshTokenExpiryTime <= DateTime.UtcNow) return null!;

			return user;
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
	}
}

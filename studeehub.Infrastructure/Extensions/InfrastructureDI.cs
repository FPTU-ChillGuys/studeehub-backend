using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using studeehub.Domain.Entities;
using studeehub.Persistence.Context;

namespace studeehub.Infrastructure.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
            // - DBContext
            var connectionString = configuration["DATABASE_CONNECTION_STRING"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("DATABASE_CONNECTION_STRING is not configured.");
            }

            services.AddDbContext<StudeeHubDBContext>(options =>
                options.UseSqlServer(connectionString));

            // - Identity
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<StudeeHubDBContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24); // Set token lifespan to 24 hours
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+đĐăĂâÂêÊôÔơƠưƯ" +
                    "áàảãạấầẩẫậắằẳẵặéèẻẽẹếềểễệíìỉĩịóòỏõọốồổỗộớờởỡợúùủũụứừửữựýỳỷỹỵ" +
                    "ÁÀẢÃẠẤẦẨẪẬẮẰẲẴẶÉÈẺẼẸẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌỐỒỔỖỘỚỜỞỠỢÚÙỦŨỤỨỪỬỮỰÝỲỶỸỴ";
            });
            return services;
		}
	}
}

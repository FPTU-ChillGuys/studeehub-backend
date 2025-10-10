using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Infrastructure.Services;
using studeehub.Persistence.Context;
using studeehub.Persistence.Repositories;

namespace studeehub.Infrastructure.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			// Register Repositories
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<IAuthRepository, AuthRepository>();
			services.AddScoped<IWorkSpaceRepository, WorkspaceRepository>();
			services.AddScoped<IStreakRepository, StreakRepository>();
			services.AddScoped<IScheduleRepository, ScheduleRepository>();
			services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

			// Register UnitOfWork
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			// Register Third-Party Services (e.g., Email, SMS)
			services.AddTransient<IEmailService, EmailService>();
			services.AddTransient<IEmailTemplateService, EmailTemplateService>();
			services.AddTransient<ISupabaseStorageService, SupabaseStorageService>();
			services.AddTransient<IVnPayService, VnPayService>();
			services.AddTransient<ISubscriptionJobService, SubscriptionJobService>();

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

			// - CORS
			var webUrl = configuration["Front-end:webUrl"] ?? throw new Exception("Missing web url!!");
			services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontend", builder =>
				{
					builder
						.WithOrigins(webUrl)
						.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowCredentials();
				});
			});
			//services.AddCors(options =>
			//{
			//	options.AddPolicy("AllowAll",
			//		policy => policy
			//			.AllowAnyMethod()
			//			.AllowAnyHeader()
			//			.SetIsOriginAllowed(_ => true)
			//			.AllowCredentials());
			//});

			// Add hangfire client
			services.AddHangfire(config =>
			{
				config
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseSqlServerStorage(connectionString);
			});
			// Add hangfire server
			services.AddHangfireServer();

			// Register job + scheduler
			services.AddScoped<ISendReminderJobService, SendReminderJobService>();
			services.AddHostedService<StartupJobScheduler>();
			return services;
		}
	}
}

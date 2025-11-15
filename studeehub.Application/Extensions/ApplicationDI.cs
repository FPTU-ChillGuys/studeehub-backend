using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Requests.Auth;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Requests.Schedule;
using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Requests.Subscription;
using studeehub.Application.DTOs.Requests.User;
using studeehub.Application.DTOs.Requests.UserAchievem;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Mappings;
using studeehub.Application.Services;
using studeehub.Application.Validators.AchievemValidators;
using studeehub.Application.Validators.AuthValidators;
using studeehub.Application.Validators.PayOsValidators;
using studeehub.Application.Validators.PomodoroSettingValidators;
using studeehub.Application.Validators.ScheduleValidators;
using studeehub.Application.Validators.StreakValidators;
using studeehub.Application.Validators.SubscriptionValidators;
using studeehub.Application.Validators.UserAchievemValidators;
using studeehub.Application.Validators.UserValidators;

namespace studeehub.Application.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			// Add application services here, e.g. MediatR, AutoMapper, etc.
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IStreakService, StreakService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAchievementService, AchievementService>();
			services.AddScoped<IUserAchievementService, UserAchievementService>();
			services.AddScoped<IScheduleService, ScheduleService>();
			services.AddScoped<IPayTransactionService, PayTransactionService>();
			services.AddScoped<ISubPlanService, SubPlanService>();
			services.AddScoped<ISubscriptionService, SubscriptionService>();
			services.AddScoped<IPomodoroSessionService, PomodoroSessionService>();
			services.AddScoped<IPomodoroSettingService, PomodoroSettingService>();
			services.AddScoped<IUserMetricsService, UserMetricsService>();


			// Mapster configuration: clone global settings and scan this assembly for IRegister implementations
			var config = TypeAdapterConfig.GlobalSettings.Clone();
			config.Scan(typeof(StreakRegister).Assembly);
			config.Scan(typeof(AchievementRegister).Assembly);
			config.Scan(typeof(UserAchievementRegister).Assembly);
			config.Scan(typeof(ScheduleRegister).Assembly);
			config.Scan(typeof(SubscriptionPlanRegister).Assembly);
			config.Scan(typeof(SubscriptionRegister).Assembly);
			config.Scan(typeof(PayTransactionRegister).Assembly);
			config.Scan(typeof(UserRegister).Assembly);
			config.Scan(typeof(PomodoroSessionRegister).Assembly);
			config.Scan(typeof(PomodoroSettingRegister).Assembly);

			// Register TypeAdapterConfig and Mapster IMapper (ServiceMapper)
			services.AddSingleton(config);
			services.AddScoped<IMapper, ServiceMapper>();

			// FluentValidation
			services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordValidator>();
			services.AddScoped<IValidator<CreateStreakRequest>, CreateStreakValidator>();
			services.AddScoped<IValidator<UpdateStreakRequest>, UpdateStreakValidator>();
			services.AddScoped<IValidator<GetAchievemsRequest>, GetAchievemValidator>();
			services.AddScoped<IValidator<CreateAchievemRequest>, CreateAchievemValidator>();
			services.AddScoped<IValidator<UpdateAchievemRequest>, UpdateAchievemValidator>();
			services.AddScoped<IValidator<UnlockAchivemRequest>, UnclockAchievemValidator>();
			services.AddScoped<IValidator<CreateScheduleRequest>, CreateScheduleValidator>();
			services.AddScoped<IValidator<UpdateScheduleRequest>, UpdateScheduleValidator>();
			services.AddScoped<IValidator<CreateSubPlanRequest>, CreateSubPlanValidator>();
			services.AddScoped<IValidator<UpdateSubPlanRequest>, UpdateSubPlanValidator>();
			services.AddScoped<IValidator<CreateLinkRequest>, CreatePaymentLinkRequestValidator>();
			services.AddScoped<IValidator<UpdateSettingRequest>, UpdateSettingValidator>();
			services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserValidator>();

			// SignalR
			services.AddSignalR();

			return services;
		}
	}
}

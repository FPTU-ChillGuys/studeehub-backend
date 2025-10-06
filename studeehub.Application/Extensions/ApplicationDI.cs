using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Requests.Auth;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Requests.Schedule;
using studeehub.Application.DTOs.Requests.Streak;
using studeehub.Application.DTOs.Requests.UserAchievem;
using studeehub.Application.DTOs.Requests.WorkSpace;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Mappings;
using studeehub.Application.Services;
using studeehub.Application.Validators.AchievemValidators;
using studeehub.Application.Validators.AuthValidators;
using studeehub.Application.Validators.DocumentValidators;
using studeehub.Application.Validators.NoteValidators;
using studeehub.Application.Validators.ScheduleValidators;
using studeehub.Application.Validators.StreakValidators;
using studeehub.Application.Validators.UserAchievemValidators;
using studeehub.Application.Validators.WorkSpaceValidators;

namespace studeehub.Application.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			// Add application services here, e.g. MediatR, AutoMapper, etc.
			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IWorkSpaceService, WorkSpaceService>();
			services.AddScoped<IDocumentService, DocumentService>();
			services.AddScoped<INoteService, NoteService>();
			services.AddScoped<IStreakService, StreakService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAchievementService, AchievementService>();
			services.AddScoped<IUserAchievementService, UserAchievementService>();
			services.AddScoped<IScheduleService, ScheduleService>();

			// Mapster configuration: clone global settings and scan this assembly for IRegister implementations
			var config = TypeAdapterConfig.GlobalSettings.Clone();
			config.Scan(typeof(WorkSpaceRegister).Assembly);
			config.Scan(typeof(DocumentRegister).Assembly);
			config.Scan(typeof(NoteRegister).Assembly);
			config.Scan(typeof(StreakRegister).Assembly);
			config.Scan(typeof(AchievementRegister).Assembly);
			config.Scan(typeof(UserAchievementRegister).Assembly);
			config.Scan(typeof(ScheduleRegister).Assembly);

			// Register TypeAdapterConfig and Mapster IMapper (ServiceMapper)
			services.AddSingleton(config);
			services.AddScoped<IMapper, ServiceMapper>();

			// FluentValidation
			services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordValidator>();
			services.AddScoped<IValidator<CreateWorkSpaceRequest>, CreateWorkSpaceValidator>();
			services.AddScoped<IValidator<UpdateWorkSpaceRequest>, UpdateWorkSpaceValidator>();
			services.AddScoped<IValidator<CreateDocumentRequest>, CreateDocumentValidator>();
			services.AddScoped<IValidator<UpdateDocumentRequest>, UpdateDocumentValidator>();
			services.AddScoped<IValidator<CreateNoteRequest>, CreateNoteValidator>();
			services.AddScoped<IValidator<UpdateNoteRequest>, UpdateNoteValidator>();
			services.AddScoped<IValidator<CreateStreakRequest>, CreateStreakValidator>();
			services.AddScoped<IValidator<UpdateStreakRequest>, UpdateStreakValidator>();
			services.AddScoped<IValidator<CreateAchievemRequest>, CreateAchievemValidator>();
			services.AddScoped<IValidator<UpdateAchievemRequest>, UpdateAchievemValidator>();
			services.AddScoped<IValidator<UnlockAchivemRequest>, UnclockAchievemValidator>();
			services.AddScoped<IValidator<CreateScheduleRequest>, CreateScheduleValidator>();
			services.AddScoped<IValidator<UpdateScheduleRequest>, UpdateScheduleValidator>();

			// SignalR
			services.AddSignalR();

			return services;
		}
	}
}

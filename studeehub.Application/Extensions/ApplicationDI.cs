using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using studeehub.Application.DTOs.Requests.Document;
using studeehub.Application.DTOs.Requests.Note;
using studeehub.Application.DTOs.Requests.WorkSpace;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Mappings;
using studeehub.Application.Services;
using studeehub.Application.Validators.DocumentValidators;
using studeehub.Application.Validators.NoteValidators;
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

			// Mapster configuration: clone global settings and scan this assembly for IRegister implementations
			var config = TypeAdapterConfig.GlobalSettings.Clone();
			config.Scan(typeof(WorkSpaceRegister).Assembly);
			config.Scan(typeof(DocumentRegister).Assembly);
			config.Scan(typeof(NoteRegister).Assembly);

			// Register TypeAdapterConfig and Mapster IMapper (ServiceMapper)
			services.AddSingleton(config);
			services.AddScoped<IMapper, ServiceMapper>();

			// FluentValidation
			services.AddScoped<IValidator<CreateWorkSpaceRequest>, CreateWorkSpaceValidator>();
			services.AddScoped<IValidator<CreateDocumentRequest>, CreateDocumentValidator>();
			services.AddScoped<IValidator<CreateNoteRequest>, CreateNoteValidator>();
			services.AddScoped<IValidator<UpdateNoteRequest>, UpdateNoteValidator>();

			return services;
		}
	}
}

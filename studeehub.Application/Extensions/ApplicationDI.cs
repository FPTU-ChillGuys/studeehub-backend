using Microsoft.Extensions.DependencyInjection;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Services;

namespace studeehub.Application.Extensions
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
            // Add application services here, e.g. MediatR, AutoMapper, etc.
            services.AddScoped<IAuthService, AuthService>();

            return services;
		}
	}
}

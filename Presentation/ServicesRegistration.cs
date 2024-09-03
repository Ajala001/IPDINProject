using App.Application;
using App.Core;
using App.Core.Interfaces;
using App.Infrastructure;

namespace App.Presentation
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAppEnvironment, AppEnvironment>();

            services.AddApplication()
            .AddCore()
            .AddInfrastructure(configuration);
            return services;
        }
    }
}



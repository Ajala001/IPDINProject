using App.Application.AuthPolicy;
using App.Application.Services;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace App.Application
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServicesRegistration).Assembly));

            services.AddScoped<IAcademicQualificationService, AcademicQualificationService>();
            services.AddScoped<IAppApplicationService, AppApplicationService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IExaminationService, ExaminationService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITrainingService, TrainingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRegistrationTypeService, RegistrationTypeService>();

            services.AddHostedService<TrainingStatusUpdaterService>();

           

            services.AddTransient<IAuthorizationHandler, PaymentHandler>();


            return services;
        }
    }
}

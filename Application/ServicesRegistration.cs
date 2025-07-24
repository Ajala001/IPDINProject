using App.Application.AuthPolicy;
using App.Application.HtmlFormat;
using App.Application.Services;
using App.Core.Interfaces.Services;
using DinkToPdf.Contracts;
using DinkToPdf;
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
            services.AddScoped<IBatchResultService, BatchResultService>();
            services.AddScoped<IExaminationService, ExaminationService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITrainingService, TrainingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<IApplicationSlip, ApplicationSlip>();
            services.AddScoped<IResultFormat, ResultFormat>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            //services.AddHostedService<TrainingStatusUpdaterService>();

           

            services.AddScoped<IAuthorizationHandler, PaymentHandler>();


            return services;
        }
    }
}

using App.Application.IExternalServices;
using App.Application.Services;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using App.Infrastructure.Data;
using App.Infrastructure.ExternalServices;
using App.Infrastructure.Persistence;
using App.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using sib_api_v3_sdk.Api;
namespace App.Infrastructure
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IPDINDbContext>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("IPDINConnectionString")!);
            });

            services.AddHttpContextAccessor();

            var brevoConfig = new sib_api_v3_sdk.Client.Configuration
            {
                ApiKey = new Dictionary<string, string>
                {
                    { "api-key", configuration["BrevoEmailApi:ApiKey"]! }
                }
            };

            services.AddSingleton(brevoConfig);
            services.AddScoped<TransactionalEmailsApi>(provider => 
                    new TransactionalEmailsApi(provider.GetRequiredService<sib_api_v3_sdk.Client.Configuration>()));



            services.AddScoped<IdentitySeeder>();


            // Register Repositories
            services.AddScoped<IAcademicQualificationRepository, AcademicQualificationRepository>();
            services.AddScoped<IAppApplicationRepository, AppApplicationRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IExaminationRepository, ExaminationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<ITrainingRepository, TrainingRepository>();
            services.AddScoped<ILevelRepository, LevelRepository>();
            services.AddScoped<IBatchResultRepository, BatchResultRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMembershipNumberCounters, MembershipNumberCounter>();
            services.AddHttpContextAccessor();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddScoped<IAuthService, AuthService>();

            


            // Register other infrastructure services here...

            return services;
        }
    }
}

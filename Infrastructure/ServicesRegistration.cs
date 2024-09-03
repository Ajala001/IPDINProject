using App.Core.Interfaces;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using App.Infrastructure.ExternalServices.Authentication;
using App.Infrastructure.ExternalServices.Email;
using App.Infrastructure.ExternalServices.Payments;
using App.Infrastructure.Identity;
using App.Infrastructure.Persistence;
using App.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace App.Infrastructure
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IPDINDbContext>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("IPDINConnectionString"));
            });

            services.AddScoped<IdentitySeeder>();

            // Register Repositories
            services.AddScoped<IAcademicQualificationRepository, AcademicQualificationRepository>();
            services.AddScoped<IAppApplicationRepository, AppApplicationRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IExaminationRepository, ExaminationRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<ITrainingRepository, TrainingRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            


            // Register other infrastructure services here...

            return services;
        }
    }
}

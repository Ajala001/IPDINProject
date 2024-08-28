using App.Core.Entities;
using App.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace App.Infrastructure.Data
{
    public class IPDINDbContext : IdentityDbContext<User, Role, Guid>
    {
        public IPDINDbContext(DbContextOptions<IPDINDbContext> options) : base(options)
        {
        }

        public DbSet<AppApplication> Applications { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<AcademicQualification> AcademicQualifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure JSON type for Breakdown in Result entity
            builder.Entity<Result>()
                .Property(r => r.Breakdown)
                .HasColumnType("json");

            // Configure keys for entities
            builder.Entity<User>().HasKey(u => u.Id);
            builder.Entity<Course>().HasKey(c => c.Id);
            builder.Entity<AppApplication>().HasKey(a => a.Id);
            builder.Entity<Payment>().HasKey(p => p.Id);
            builder.Entity<Result>().HasKey(r => r.Id);
            builder.Entity<Training>().HasKey(t => t.Id);
            builder.Entity<Examination>().HasKey(e => e.Id);
            builder.Entity<AcademicQualification>().HasKey(aq => aq.Id);

            // Configure 1-to-Many relationships
            builder.Entity<Course>()
                .HasMany(c => c.Examinations)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId);

            builder.Entity<Examination>()
                .HasMany(e => e.Results)
                .WithOne(r => r.Examination)
                .HasForeignKey(r => r.ExaminationId);

            builder.Entity<User>()
                .HasMany(u => u.Payments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            builder.Entity<User>()
                .HasMany(u => u.Results)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            builder.Entity<User>()
                .HasMany(u => u.Applications)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);


            // Configure Many-to-Many relationships
            // User and Course
            builder.Entity<UserCourses>()
                .HasKey(uc => new { uc.UserId, uc.CourseId });

            builder.Entity<UserCourses>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Courses)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserCourses>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.Courses)
                .HasForeignKey(uc => uc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // User and Examination
            builder.Entity<UserExaminations>()
                .HasKey(ue => new { ue.UserId, ue.ExaminationId });

            builder.Entity<UserExaminations>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.Examinations)
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserExaminations>()
                .HasOne(ue => ue.Examination)
                .WithMany(e => e.Examinations)
                .HasForeignKey(ue => ue.ExaminationId)
                .OnDelete(DeleteBehavior.Cascade);

            // User and Training
            builder.Entity<UserTrainings>()
                .HasKey(ut => new { ut.UserId, ut.TrainingId });

            builder.Entity<UserTrainings>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.Trainings)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserTrainings>()
                .HasOne(ut => ut.Training)
                .WithMany(t => t.Trainings)
                .HasForeignKey(ut => ut.TrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            // User and AcademicQualification
            builder.Entity<UserAcademicQualifications>()
                .HasKey(uaq => new { uaq.UserId, uaq.QualificationId });

            builder.Entity<UserAcademicQualifications>()
                .HasOne(uaq => uaq.User)
                .WithMany(u => u.UserAcademicQualifications)
                .HasForeignKey(uaq => uaq.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserAcademicQualifications>()
                .HasOne(uaq => uaq.Qualification)
                .WithMany(aq => aq.UserAcademicQualifications)
                .HasForeignKey(uaq => uaq.QualificationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


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
        public DbSet<BatchResult> BatchResults { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<MembershipNumberCounter> MembershipNumberCounters { get; set; }
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

            // Configure 1-to-1 relationships
            builder.Entity<Examination>()
               .HasOne(e => e.BatchResult)        
               .WithOne(br => br.Examination)       
               .HasForeignKey<BatchResult>(br => br.ExaminationId);  

            // Configure 1-to-Many relationships
            builder.Entity<User>()
                .HasMany(u => u.Payments)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            builder.Entity<BatchResult>()
                .HasMany(br => br.Results)
                .WithOne(r => r.BatchResult)
                .HasForeignKey(r => r.BatchId);

            builder.Entity<User>()
                .HasMany(u => u.Results)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            builder.Entity<User>()
                .HasMany(u => u.Applications)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            builder.Entity<User>()
            .HasOne(u => u.Level)  // Each User has one RegistrationType
            .WithMany(rt => rt.Users)         // Each RegistrationType has many Users
            .HasForeignKey(u => u.LevelId)  // Foreign key in User table
            .OnDelete(DeleteBehavior.Restrict);


            // Configure Many-to-Many relationships
            //Corse  and Examination
            builder.Entity<Examination>()
                .HasMany(e => e.Courses)
                .WithMany(c => c.Examinations)
                .UsingEntity(j => j.ToTable("ExamCourses")); // This defines the join table


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


using App.Core.Enums;
using Microsoft.AspNetCore.Identity;
namespace App.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public Guid RegistrationTypeId { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public string? MembershipNumber { get; set; }
        public bool HasPaidDues { get; set; } = false;
        public required Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfilePic { get; set; }
        public RegistrationType? RegistrationType { get; set; }
        public short? StreetNo { get; set; }
        public string? StreetName { get; set; }
        public string? City { get; set; }
        public required string StateOfResidence { get; set; }
        public string? LocalGovt { get; set; }
        public string? StateOfOrigin { get; set; }
        public required string Country { get; set; }
        public required string DriverLicenseNo { get; set; }
        public short YearIssued { get; set; }
        public DateTime ExpiringDate { get; set; }
        public short YearsOfExperience { get; set; }
        public required string NameOfCurrentDrivingSchool { get; set; }
        public ICollection<Result> Results { get; set; } = new List<Result>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<UserCourses> Courses { get; set; } = new List<UserCourses>();
        public ICollection<UserTrainings> Trainings { get; set; } = new List<UserTrainings>();
        public ICollection<AppApplication> Applications { get; set; } = new List<AppApplication>();
        public ICollection<UserExaminations> Examinations { get; set; } = new List<UserExaminations>();
        public ICollection<UserAcademicQualifications> UserAcademicQualifications { get; set; } = new List<UserAcademicQualifications>();
        public DateTime CreatedOn { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }

    }

}



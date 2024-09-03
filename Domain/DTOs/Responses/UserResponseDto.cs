using App.Core.Entities;
using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public List<string>? RoleNames { get; set; }
        public string? MembershipNumber { get; set; }
        public required Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfilePic { get; set; }
        public UserCategory? Category { get; set; }
        public string Address { get; set; } = null!;
        public string LocalGovt { get; set; } = null!;
        public string StateOfOrigin { get; set; } = null!;
        public string DriverLicenseNo { get; set; } = null!;
        public short? YearIssued { get; set; }
        public DateOnly? ExpiringDate { get; set; }
        public short? YearsOfExperience { get; set; }
        public string NameOfCurrentDrivingSchool { get; set; } = null!;
        public IEnumerable<AcademicQualificationInfo>? AcademicQualifications { get; set; }
    }
}

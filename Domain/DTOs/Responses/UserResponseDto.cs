using App.Core.Entities;
using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? MembershipNumber { get; set; }
        public required Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfilePic { get; set; }
        public UserCategory? Category { get; set; }
        public string Address { get; set; } = null!;
        public required string LocalGovt { get; set; }
        public required string StateOfOrigin { get; set; }
        public required string DriverLicenseNo { get; set; }
        public required short YearIssued { get; set; }
        public required DateOnly ExpiringDate { get; set; }
        public short YearsOfExperience { get; set; }
        public required string NameOfCurrentDrivingSchool { get; set; }
        public IEnumerable<AcademicQualificationInfo> AcademicQualifications { get; set; } = new List<AcademicQualificationInfo>();
    }
}

using App.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateUserRequestDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IFormFile? ProfilePic { get; set; }
        public short? StreetNo { get; set; }
        public string? StreetName { get; set; }
        public string? City { get; set; }
        public string? StateOfResidence { get; set; }
        public string? LocalGovt { get; set; }
        public string? StateOfOrigin { get; set; }
        public string? Country { get; set; }
        public string? DriverLicenseNo { get; set; }
        public short? YearIssued { get; set; }
        public DateTime? ExpiringDate { get; set; }
        public short? YearsOfExperience { get; set; }
        public string? NameOfCurrentDrivingSchool { get; set; }
    }
}

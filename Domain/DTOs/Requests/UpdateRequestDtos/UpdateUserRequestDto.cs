﻿using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.Enums;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateUserRequestDto
    {
        public class UpdateUserDto
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public Gender? Gender { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? ProfilePic { get; set; }
            public UserCategory? Category { get; set; }
            public short? StreetNo { get; set; }
            public string? StreetName { get; set; }
            public string? City { get; set; }
            public string? StateOfResidence { get; set; }
            public string? LocalGovt { get; set; }
            public string? StateOfOrigin { get; set; }
            public string? Country { get; set; }
            public string? DriverLicenseNo { get; set; }
            public short? YearIssued { get; set; }
            public DateOnly? ExpiringDate { get; set; }
            public short? YearsOfExperience { get; set; }
            public string? NameOfCurrentDrivingSchool { get; set; }
            public IEnumerable<CreateAcademicQualificationRequestDto>? AcademicQualifications { get; set; }
        }
    }
}
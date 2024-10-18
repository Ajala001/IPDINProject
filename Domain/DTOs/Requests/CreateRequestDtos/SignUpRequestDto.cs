using App.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class SignUpRequestDto
    {
        public required Guid LevelId { get; set; }
        [Required(ErrorMessage = "Please Enter Your Last Name")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Your First Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        [EmailAddress(ErrorMessage = "Enter A Valid Email")]
        public required string Email { get; set; }


        [Required(ErrorMessage = "Please Enter Your Phone Number")]
        public required string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Please Enter A Strong Password")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please Confirm Your Password")]
        [Compare("Password", ErrorMessage = "Passwords Do Not Match")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please Select Your Gender")]
        public required Gender Gender { get; set; }

        [Required(ErrorMessage = "Please Enter Your Date Of Birth")]
        public required DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "What Country are you from")]
        public required string Country { get; set; }

        [Required(ErrorMessage = "What State are you from")]
        public required string StateOfResidence { get; set; }

        [Required(ErrorMessage = "Please Enter your driver license number")]
        public required string DriverLicenseNo { get; set; }

        [Required(ErrorMessage = "What Year was it issued")]
        public required short YearIssued { get; set; }

        [Required(ErrorMessage = "Enter the Expiring Date")]
        public required DateTime ExpiringDate { get; set; }

        [Required(ErrorMessage = "Your Years of Experience")]
        public required short YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Please Enter your Current driving school")]
        public required string NameOfCurrentDrivingSchool { get; set; }

        [Required(ErrorMessage = "What Type Re you registering for")]
        public List<CreateAcademicQualificationRequestDto> AcademicQualifications { get; set; } = new();

    }

}





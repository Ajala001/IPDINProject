using App.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateUserRequestDto
    {
        [Required(ErrorMessage = "Please Enter Your Last Name")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Your First Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Your Email")]
        [EmailAddress(ErrorMessage = "Enter A Valid Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Please Enter A Strong Password")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please Confirm Your Password")]
        [Compare("Password", ErrorMessage = "Passwords Do Not Match")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please Enter Your Phone Number")]
        [Phone(ErrorMessage = "Enter A Valid Phone Number")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Select Your Gender")]
        public required Gender Gender { get; set; }

        [Required(ErrorMessage = "Please Enter Your Date Of Birth")]
        public required DateOnly DateOfBirth { get; set; }

        public string? ProfilePic { get; set; }

        public UserCategory? Category { get; set; }

        [Required(ErrorMessage = "Please Enter Your Street Number")]
        public required short StreetNo { get; set; }

        [Required(ErrorMessage = "Please Enter Your Street Name")]
        public required string StreetName { get; set; }

        [Required(ErrorMessage = "Please Enter Your City")]
        public required string City { get; set; }

        [Required(ErrorMessage = "Please Enter Your State Of Residence")]
        public required string StateOfResidence { get; set; }

        [Required(ErrorMessage = "Please Enter Your Local Government Area")]
        public required string LocalGovt { get; set; }

        [Required(ErrorMessage = "Please Enter Your State Of Origin")]
        public required string StateOfOrigin { get; set; }

        [Required(ErrorMessage = "Please Enter Your Country")]
        public required string Country { get; set; }

        [Required(ErrorMessage = "Please Enter Your Driver License Number")]
        public required string DriverLicenseNo { get; set; }

        [Required(ErrorMessage = "Please Enter The Year Issued")]
        public required short YearIssued { get; set; }

        [Required(ErrorMessage = "Please Enter The Expiration Date")]
        public required DateOnly ExpiringDate { get; set; }

        public short YearsOfExperience { get; set; } 

        [Required(ErrorMessage = "Please Enter The Name Of Your Current Driving School")]
        public required string NameOfCurrentDrivingSchool { get; set; }
        public required string CreatedBy { get; set; }

        [Required(ErrorMessage = "Please Provide At Least One Academic Qualification")]
        public required IEnumerable<CreateAcademicQualificationRequestDto> AcademicQualifications { get; set; }
    }

}







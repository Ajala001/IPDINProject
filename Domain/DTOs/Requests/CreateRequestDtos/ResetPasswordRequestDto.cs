using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class ResetPasswordRequestDto
    {
        public string? Token { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please Enter A Strong Password")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords Do Not Match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set;}
    }
}

using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class ChangePasswordRequestDto
    {
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Enter your current password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "Please Enter A Strong Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;
    }
}

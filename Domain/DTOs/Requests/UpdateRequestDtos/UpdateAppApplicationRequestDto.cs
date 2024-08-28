using App.Core.Entities;
using App.Core.Enums;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateAppApplicationRequestDto
    {
        public string? ApplicationPurpose { get; set; } // reason for making the application
        public ApplicationStatus? Status { get; set; }
    }
}

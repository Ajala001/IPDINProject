using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class AppApplicationResponseDto
    {
        public Guid Id { get; set; }
        public string ApplicantFullName { get; set; } = null!;
        public string? ApplicationPurpose { get; set; } // reason for making the application
        public DateTime Date { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}

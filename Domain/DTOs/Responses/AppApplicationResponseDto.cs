using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class AppApplicationResponseDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public string ApplicantFullName { get; set; } = string.Empty;
        public string AppliedFor { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string HasPaid { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
    }
}

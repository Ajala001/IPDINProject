using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class TrainingResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public required decimal Fee { get; set; }
        public DateTime StartingDateAndTime { get; set; }
        public DateTime EndingDateAndTime { get; set; }
        public DateTime RegistrationDeadline { get; set; }
        public int Duration { get; set; }
        public int Capacity { get; set; }
        public TrainingCategory Category { get; set; }
        public TrainingStatus Status { get; set; }
    }
}

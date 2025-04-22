using App.Core.Enums;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateTrainingRequestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Fee { get; set; }
        public DateTime? StartingDateAndTime { get; set; }
        public DateTime? EndingDateAndTime { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public int? Duration { get; set; }
        public int? Capacity { get; set; }
        public TrainingCategory? Category { get; set; }
        public TrainingStatus? Status { get; set; }
    }
}

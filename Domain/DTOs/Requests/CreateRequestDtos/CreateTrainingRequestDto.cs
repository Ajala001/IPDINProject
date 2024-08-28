using App.Core.Enums;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateTrainingRequestDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required decimal Fee { get; set; }
        public required DateTime StartingDateAndTime { get; set; }
        public required DateTime EndingDateAndTime { get; set; }
        public required DateTime RegistrationDeadline { get; set; }
        public required int Duration { get; set; }
        public required int Capacity { get; set; }
        public TrainingCategory Category { get; set; }
    }
}

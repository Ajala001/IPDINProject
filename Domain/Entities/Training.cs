using App.Core.Enums;

namespace App.Core.Entities
{
    public class Training : Auditables
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required decimal Fee { get; set; }
        public decimal ApplicationFee { get; set; }
        public bool Haspaid { get; set; } = false;
        public required DateTime StartingDateAndTime { get; set; }
        public required DateTime EndingDateAndTime { get; set; }
        public required DateTime RegistrationDeadline { get; set; }
        public required int Duration { get; set; }
        public required int Capacity { get; set; }
        public TrainingCategory Category { get; set; }
        public TrainingStatus Status { get; set; }
        public ICollection<UserTrainings> Trainings { get; set; } = new List<UserTrainings>();
    }
}
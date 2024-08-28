namespace App.Core.Entities
{
    public class UserTrainings
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TrainingId { get; set; }
        public required User User { get; set; }
        public required Training Training { get; set; }
    }
}

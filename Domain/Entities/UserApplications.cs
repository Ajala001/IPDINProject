using App.Core.Interfaces;

namespace App.Core.Entities
{
    public class UserApplications
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public required User User { get; set; }
        public required AppApplication Application { get; set; }
    }
}

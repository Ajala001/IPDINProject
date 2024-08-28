using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}

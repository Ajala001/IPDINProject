using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.Entities
{
    public class Result : Auditables
    {
        public required Guid UserId { get; set; }
        public required Guid BatchId { get; set; }
        public required double TotalScore { get; set; }
        [Column(TypeName = "json")]
        public required string Breakdown { get; set; }
        public required User User { get; set; }
        public required BatchResult BatchResult { get; set; }

    }
}

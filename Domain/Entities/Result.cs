using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.Entities
{
    public class Result : Auditables
    {
        public required Guid UserId { get; set; }
        public required Guid ExaminationId { get; set; }
        public required int TotalScore { get; set; }
        [Column(TypeName = "json")]
        public required string Breakdown { get; set; }
        public required User User { get; set; }
        public required Examination Examination { get; set; }
    }
}

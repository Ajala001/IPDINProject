using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateResultRequestDto
    {
        public Guid? UserId { get; set; }
        public Guid? ExaminationId { get; set; }
        public int? TotalScore { get; set; }
        [Column(TypeName = "json")]
        public string? Breakdown { get; set; }
    }
}

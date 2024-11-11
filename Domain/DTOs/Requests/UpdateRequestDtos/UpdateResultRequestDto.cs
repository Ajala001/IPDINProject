using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateResultRequestDto
    {
        public Guid ExaminationId { get; set; }
        public double? TotalScore { get; set; }
        [Column(TypeName = "json")]
        public string? Breakdown { get; set; }
    }
}

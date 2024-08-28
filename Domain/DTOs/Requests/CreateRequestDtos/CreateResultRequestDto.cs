using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateResultRequestDto
    {
        public required Guid UserId { get; set; }
        public required Guid ExaminationId { get; set; }
        public required int TotalScore { get; set; }
        [Column(TypeName = "json")]
        public required string Breakdown { get; set; }
    }
}

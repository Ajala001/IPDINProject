using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.DTOs.Responses
{
    public class ResultResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string ExamTitle { get; set; } = null!;
        public int TotalScore { get; set; }
        [Column(TypeName = "json")]
        public required string Breakdown { get; set; }
    }
}

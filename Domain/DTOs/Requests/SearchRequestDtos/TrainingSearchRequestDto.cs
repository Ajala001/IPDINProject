using App.Core.Enums;

namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public class TrainingSearchRequestDto
    {
        public string? TrainingTitle { get; set; }
        public TrainingCategory? Category { get; set; }
        public TrainingStatus? Status { get; set; } 
    }
}

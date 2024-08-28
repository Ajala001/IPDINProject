namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateExaminationRequestDto
    {
        public Guid? CourseId { get; set; }
        public string? ExamTitle { get; set; }
        public decimal? Fee { get; set; }
        public DateTime? ExamDateAndTime { get; set; }
    }
}

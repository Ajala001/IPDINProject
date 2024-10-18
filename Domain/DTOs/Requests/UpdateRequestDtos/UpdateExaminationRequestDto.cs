using App.Core.Enums;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateExaminationRequestDto
    {
        public string? ExamTitle { get; set; }
        public short? ExamYear { get; set; }
        public int? Fee { get; set; }
        public ExaminationStatus? Status { get; set; }
        public DateTime? ExamDateAndTime { get; set; }
        public List<Guid> SelectedCourses { get; set; } = new();
    }
}

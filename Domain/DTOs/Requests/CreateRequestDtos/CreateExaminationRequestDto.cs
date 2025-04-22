using App.Core.Enums;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateExaminationRequestDto
    {
        public required string ExamTitle { get; set; }
        public DateTime ExamDateAndTime { get; set; }
        public short ExamYear { get; set; }
        public required decimal Fee { get; set; }
        public ExaminationStatus Status { get; set; }
        public ICollection<Guid> Courses { get; set; } = new List<Guid>();

    }
}

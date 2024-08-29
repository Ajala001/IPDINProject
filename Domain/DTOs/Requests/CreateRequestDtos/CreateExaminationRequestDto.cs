using App.Core.Entities;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateExaminationRequestDto
    {
        public required string ExamTitle { get; set; }
        public DateTime ExamDateAndTime { get; set; }
        public DateOnly ExamYear { get; set; }
        public required decimal Fee { get; set; }
        public required Guid CourseId { get; set; }
    }
}

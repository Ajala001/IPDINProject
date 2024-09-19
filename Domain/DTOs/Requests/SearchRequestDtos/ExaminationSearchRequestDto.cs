namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public class ExaminationSearchRequestDto
    {
        public short? ExamYear { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseTitle { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}


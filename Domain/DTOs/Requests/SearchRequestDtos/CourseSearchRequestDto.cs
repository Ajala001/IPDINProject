namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public record CourseSearchRequestDto
    {
        public string? CourseTitle { get; init; }
        public string? CourseCode { get; init; }
    }
}

namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public class ExaminationSearchRequestDto
    {
        public string? SearchQuery { get; init; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}


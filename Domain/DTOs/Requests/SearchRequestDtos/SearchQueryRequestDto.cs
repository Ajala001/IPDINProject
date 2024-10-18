using App.Core.Enums;

namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public record SearchQueryRequestDto
    {
        public string? SearchQuery { get; init; } 
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}

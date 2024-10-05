namespace App.Core.DTOs.Responses
{
    public class PagedResponse<T>
    {
        public T? Data { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

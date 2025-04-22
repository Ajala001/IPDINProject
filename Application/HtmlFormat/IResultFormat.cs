using App.Core.Entities;

namespace App.Application.HtmlFormat
{
    public interface IResultFormat
    {
        Task<string> HtmlContent(Result result);
    }
}

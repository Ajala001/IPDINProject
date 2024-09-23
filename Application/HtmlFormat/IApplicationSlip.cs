using App.Core.Entities;

namespace App.Application.HtmlFormat
{
    public interface IApplicationSlip
    {
        Task<string> HtmlContent(AppApplication appApplication);
    }
}

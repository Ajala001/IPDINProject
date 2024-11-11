using Microsoft.AspNetCore.Http;

namespace App.Core.Interfaces.Repositories
{
    public interface IFileRepository
    {
        Task<string> UploadAsync(IFormFile file, HttpRequest request);
    }
}

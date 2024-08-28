using App.Core.Interfaces;
using App.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace App.Infrastructure.Repositories
{
    public class FileRepository(IAppEnvironment appEnvironment) : IFileRepository
    {
        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploads = Path.Combine(appEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                var newFileName = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploads, newFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return $"/uploads/{newFileName}";
                }
                catch (Exception ex)
                {
                    // Log the exception (ex) here
                    return null;
                }
            }
            return null;
        }
    }
}

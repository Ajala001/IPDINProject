using App.Core.Interfaces;

namespace App.Presentation
{
    public class AppEnvironment(IWebHostEnvironment webHostEnvironment) : IAppEnvironment
    {
        public string WebRootPath => webHostEnvironment.WebRootPath;

        public string ContentRootPath => webHostEnvironment.ContentRootPath;
    }
}

namespace App.Core.Interfaces
{
    public interface IAppEnvironment
    {
        string WebRootPath { get; }
        string ContentRootPath { get; }
    }
}

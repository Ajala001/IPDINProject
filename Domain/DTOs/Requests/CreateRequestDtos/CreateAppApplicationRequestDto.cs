namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateAppApplicationRequestDto
    {
        public Guid ApplicationId { get; set; } 
        public string ApplicationType { get; set; } = String.Empty;  //Training, Examination
    }
}

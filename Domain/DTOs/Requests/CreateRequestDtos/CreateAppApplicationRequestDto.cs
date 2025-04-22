namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateAppApplicationRequestDto
    {
        public Guid ServiceId { get; set; }
        public bool IsTraining { get; set; }
    }
}

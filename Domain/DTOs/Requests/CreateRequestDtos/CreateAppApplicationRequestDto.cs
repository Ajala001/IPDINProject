namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateAppApplicationRequestDto
    {
        public Guid? ExaminationId { get; set; }
        public Guid? TrainingId { get; set; }
    }
}

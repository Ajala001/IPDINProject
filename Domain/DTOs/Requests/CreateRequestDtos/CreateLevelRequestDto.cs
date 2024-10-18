namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateLevelRequestDto
    {
        public required string Name { get; set; }
        public required int Dues { get; set; }
    }
}

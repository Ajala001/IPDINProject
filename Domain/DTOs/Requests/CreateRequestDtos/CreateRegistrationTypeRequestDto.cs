namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateRegistrationTypeRequestDto
    {
        public required string Name { get; set; }
        public required int Dues { get; set; }
    }
}

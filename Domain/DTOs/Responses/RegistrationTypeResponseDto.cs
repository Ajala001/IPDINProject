namespace App.Core.DTOs.Responses
{
    public class RegistrationTypeResponseDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Dues { get; set; }
    }
}

namespace App.Core.DTOs.Responses
{
    public class LevelRequestResponseDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Dues { get; set; }
    }
}

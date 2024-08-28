namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateRoleRequestDto
    {
        public required string RoleName { get; set; }
        public string? Description { get; set; }
    }
}

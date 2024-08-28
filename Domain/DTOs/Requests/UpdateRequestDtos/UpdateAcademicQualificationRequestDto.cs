namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateAcademicQualificationRequestDto
    {
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public string? Institution { get; set; }
        public short? YearAttained { get; set; }
    }
}

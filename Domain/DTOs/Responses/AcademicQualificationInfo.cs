namespace App.Core.DTOs.Responses
{
    public record AcademicQualificationInfo
    {
        public string Degree { get; init; } = null!; // Degree obtained (e.g., BSc, MSc, PhD)
        public string FieldOfStudy { get; init; } = null!; // Field of study (e.g., Computer Science, Mathematics)
    }
}

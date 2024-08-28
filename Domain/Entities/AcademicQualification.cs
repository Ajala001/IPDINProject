namespace App.Core.Entities
{
    public class AcademicQualification : Auditables
    {
        public required string Degree { get; set; } // The degree obtained (e.g., BSc, MSc, PhD)
        public required string FieldOfStudy { get; set; } // The field of study (e.g., Computer Science, Mathematics)
        public required string Institution { get; set; }
        public required short YearAttained { get; set; } // Changed to short or int for numeric year representation
        public ICollection<UserAcademicQualifications> UserAcademicQualifications { get; set; } = new List<UserAcademicQualifications>();
    }

}

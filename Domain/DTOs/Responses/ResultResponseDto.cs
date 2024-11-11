using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.DTOs.Responses
{
    public class BulkResultResponseDto
    {
        public Guid Id { get; set; } // Unique ID for the bulk upload
        public string ExamTitle { get; set; } = null!;
        public string UploadDate { get; set; } = null!;
        public int TotalStudents { get; set; } // Total number of students in this bulk upload
        public List<StudentResultResponseDto> StudentResults { get; set; } = new(); // List of individual student results
    }


    public class StudentResultResponseDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string ExamTitle { get; set; } = null!;
        public double TotalScore { get; set; }
        [Column(TypeName = "json")]
        public required string Breakdown { get; set; }
    }
}

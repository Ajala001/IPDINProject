using System.ComponentModel.DataAnnotations.Schema;

namespace App.Core.DTOs.Responses
{
    public class BulkResultResponseDto
    {
        public Guid Id { get; set; } // Unique ID for the bulk upload
        public Guid ExamId { get; set; } 
        public string ExamTitle { get; set; } = null!;
        public string UploadDate { get; set; } = null!;
        public int TotalStudents { get; set; } // Total number of students in this bulk upload
        public List<StudentResultResponseDto> StudentResults { get; set; } = new(); // List of individual student results
    }


    public class StudentResultResponseDto
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public Guid ExamId { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string MembershipNumber { get; set; } = null!;
        public string ProfilePic { get; set; } = null!; 
        public double TotalScore { get; set; }
        [Column(TypeName = "json")]
        public required string Breakdown { get; set; }
    }
}

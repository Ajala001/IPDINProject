using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Responses
{
    public class AcademicQualificationResponseDto
    {
        public Guid Id { get; set; } // Required for identifying the entity
        public string Degree { get; set; } = null!; // The degree obtained (e.g., BSc, MSc, PhD)
        public string FieldOfStudy { get; set; } = null!; // The field of study (e.g., Computer Science, Mathematics)
        public string Institution { get; set; } = null!;
        public short YearAttained { get; set; }
    }
}

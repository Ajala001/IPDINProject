using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateAcademicQualificationRequestDto
    {
        public required string Degree { get; set; } // The degree obtained (e.g., BSc, MSc, PhD)
        public required string FieldOfStudy { get; set; } // The field of study (e.g., Computer Science, Mathematics)
        public required string Institution { get; set; }
        public required short YearAttained { get; set; } 
    }
}

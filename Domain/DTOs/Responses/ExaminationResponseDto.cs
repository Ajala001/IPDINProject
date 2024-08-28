using App.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Responses
{
    public class ExaminationResponseDto
    {
        public Guid Id { get; set; }
        public string ExamTitle { get; set; } = null!;
        public DateTime ExamDateAndTime { get; set; } 
        public string CourseTitle { get; set; } = null!;
        public decimal Fee { get; set; }
    }
}

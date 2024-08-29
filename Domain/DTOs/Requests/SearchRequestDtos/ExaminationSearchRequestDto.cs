using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public class ExaminationSearchRequestDto
    {
        public DateOnly? ExamYear { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseTitle { get; set; }
    }
}

using App.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Requests.SearchRequestDtos
{
    public class TrainingSearchRequestDto
    {
        public string? TrainingTitle { get; set; }
        public TrainingCategory? Category { get; set; }
        public TrainingStatus? Status { get; set; } 
    }
}

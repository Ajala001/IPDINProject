using App.Core.Entities;
using App.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Responses
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required decimal Amount { get; set; }
        public required Guid PaymentRef { get; set; }
        public required string PaymentFor { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentStatus Status { get; set; }
    }
}


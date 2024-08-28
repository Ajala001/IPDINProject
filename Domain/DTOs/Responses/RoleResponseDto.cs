using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Responses
{
    public class RoleResponseDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<string> FullNames { get; set; } = new List<string>();
    }
}

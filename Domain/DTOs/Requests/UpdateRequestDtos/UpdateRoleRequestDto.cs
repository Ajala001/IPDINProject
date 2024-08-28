using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateRoleRequestDto
    {
        public string? RoleName { get; set; }
        public string? Description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Please Enter Your Membership Number")]
        public required string MembershipNumber { get; set; }

        [Required(ErrorMessage = "Please Enter A Strong Password")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

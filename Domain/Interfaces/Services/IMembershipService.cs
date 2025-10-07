using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interfaces.Services
{
    public interface IMembershipService
    {
        Task<string> GenerateMembershipNumberAsync(string role);
    }
}

using App.Application.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await sender.Send(new GetDashboardStatsQuery());
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }
    }
}

using App.Application.Commands.Role;
using App.Application.Queries.Role;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/role")]
    [ApiController]
    public class RoleController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddRoleAsync([FromBody] CreateRoleRequestDto request)
        {
            var result = await sender.Send(new AddRoleCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoleAsync()
        {
            var result = await sender.Send(new GetAllRoleQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{roleName}")]
        public async Task<IActionResult> GetRoleByNameAsync([FromRoute] string roleName)
        {
            var result = await sender.Send(new GetRoleByNameQuery(roleName));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpPut("{roleName}")]
        public async Task<IActionResult> UpdateRoleAsync([FromRoute] string roleName, [FromBody] UpdateRoleRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateRoleCommand(roleName, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteApplicationAsync([FromRoute] string roleName)
        {
            var result = await sender.Send(new DeleteRoleCommand(roleName));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

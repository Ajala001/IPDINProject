using App.Application.Commands.User;
using App.Application.Queries.User;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var result = await sender.Send(new GetAllUserQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmailAsync([FromRoute] string email)
        {
            var result = await sender.Send(new GetUserByEmailQuery(email));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string email, [FromBody] UpdateUserRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateUserCommand(email, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] string email)
        {
            var result = await sender.Send(new DeleteUserCommand(email));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

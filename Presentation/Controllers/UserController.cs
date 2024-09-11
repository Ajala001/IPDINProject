using App.Application.Commands.User;
using App.Application.Queries.User;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var result = await sender.Send(new GetAllUserQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserByEmailAsync([FromRoute] string email)
        {
            var result = await sender.Send(new GetUserByEmailQuery(email));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] string email, [FromBody] UpdateUserRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateUserCommand(email, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteApplicationAsync([FromRoute] string email)
        {
            var result = await sender.Send(new DeleteUserCommand(email));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

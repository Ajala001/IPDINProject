using App.Application.Commands.User;
using App.Core.DTOs.Requests.CreateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/Auths")]
    [ApiController]
    public class AuthenticationController(ISender sender) : ControllerBase
    {

        [HttpPost("signUp")] //the default route
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequestDto request)
        {
            var result = await sender.Send(new SignUpCommand(request));
            if (!result.Succeeded) return Unauthorized();
            return Ok(result.Succeeded);
        }

        [HttpPost("signIn")] //the default route
        public async Task<IActionResult> SignInAsync([FromBody] SignInRequestDto request)
        {
            var result = await sender.Send(new SignInCommand(request));
            if (result == null) return Unauthorized();
            return Ok(result);
        }


        [HttpPost("signOut")] //the default route
        public async Task<IActionResult> SignOutAsync()
        {
            await sender.Send(new SignOutCommand());
            return Ok();
        }
    }
}

using App.Application.Commands.Authentication;
using App.Core.DTOs.Requests.CreateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController(ISender sender, IConfiguration configuration) : ControllerBase
    {

        [HttpPost("signUp")] 
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequestDto request)
        {
            var result = await sender.Send(new SignUpCommand(request));
            if (!result.IsSuccessful) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInRequestDto request)
        {
            var result = await sender.Send(new SignInCommand(request));
            return Ok(result);
        }


        [HttpPost("signOut")] 
        public async Task<IActionResult> SignOutAsync()
        {
            await sender.Send(new SignOutCommand());
            return Ok();
        }


        [HttpGet("confirmEmail")] 
        public async Task<IActionResult> ConfirmEmail([FromQuery]string email, [FromQuery]string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token)) return BadRequest();
            var result = await sender.Send(new ConfirmEmailCommand(email, token));
            if(result.IsSuccessful) return Redirect($"{configuration["AngularUrl"]}/confirmation-page");
            return BadRequest(result);
        }


        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return NotFound();
            var result = await sender.Send(new ForgetPasswordCommand(email));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("resendConfirmationToken")]
        public async Task<IActionResult> ResendEmailConfirmationToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest();
            var result = await sender.Send(new ResendEmailConfirmationTokenCommand(email));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest("Something went wrong");
            var result = await sender.Send(new ResetPasswordCommand(request));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest("Something went wrong");
            var result = await sender.Send(new ChangePasswordCommand(request));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }
    }
}

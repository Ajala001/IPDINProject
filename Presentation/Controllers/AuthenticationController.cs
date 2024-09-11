using App.Application.Commands.Authentication;
using App.Core.DTOs.Requests.CreateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController(ISender sender) : ControllerBase
    {

        [HttpPost("signUp")] 
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpRequestDto request)
        {
            var result = await sender.Send(new SignUpCommand(request));
            if (!result.IsSuccessful) return BadRequest(result);
            return Ok(result.Message);
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInRequestDto request)
        {
            var result = await sender.Send(new SignInCommand(request));
            if (result == null) return BadRequest(result);
            return Ok(result);
        }


        [HttpPost("signOut")] 
        public async Task<IActionResult> SignOutAsync()
        {
            await sender.Send(new SignOutCommand());
            return Ok();
        }


        [HttpGet("confirmEmail")] 
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token)) return BadRequest();
            var result = await sender.Send(new ConfirmEmailCommand(email, token));
            if(result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest();
            var result = await sender.Send(new ForgetPasswordCommand(email));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpGet("resetPassword")]
        public IActionResult ResetPassword([FromQuery] string email, [FromQuery] string token)
        {
            var resetPasswordDto = new ResetPasswordRequestDto
            {
                Email = email,
                Token = token
            };

            return Ok(resetPasswordDto);
        }



        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest("Something went wrong");
            var result = await sender.Send(new ResetPasswordCommand(request));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }
    }
}

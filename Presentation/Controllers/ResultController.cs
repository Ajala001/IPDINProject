using App.Application.Commands.Result;
using App.Application.Queries.Result;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/results")]
    [ApiController]
    public class ResultController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllResultAsync()
        {
            var result = await sender.Send(new GetAllResultQuery());
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("uploadResult")]
        public async Task<ActionResult> UploadResultAsync(IFormFile file)
        {
            var result = await sender.Send(new UploadResultCommand(file));
            if(result.IsSuccessful) return Ok(result);
            return BadRequest(result);  
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{membershipNumber}")]
        public async Task<IActionResult> GetResultAsync([FromRoute] string membershipNumber)
        {
            var result = await sender.Send(new GetResultByMembershipNoQuery(membershipNumber));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{membershipNumber}")]
        public async Task<IActionResult> UpdateResultAsync([FromRoute] string membershipNumber, UpdateResultRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateResultCommand(membershipNumber, updateRequest));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{membershipNumber}")]
        public async Task<IActionResult> DeleteResultAsync([FromRoute] string membershipNumber)
        {
            var result = await sender.Send(new DeleteResultCommand(membershipNumber));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }
    }
}

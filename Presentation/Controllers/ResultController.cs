using App.Application.Commands.Result;
using App.Application.Queries.Result;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/result")]
    [ApiController]
    public class ResultController(ISender sender) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllResultAsync()
        {
            var result = await sender.Send(new GetAllResultQuery());
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("uploadResult")]
        public async Task<ActionResult> UploadResultAsync(IFormFile file)
        {
            var result = await sender.Send(new UploadResultCommand(file));
            if(result.IsSuccessful) return Ok(result);
            return BadRequest(result);  
        }

        [HttpGet("{membershipNumber}")]
        public async Task<IActionResult> GetResultAsync([FromRoute] string membershipNumber)
        {
            var result = await sender.Send(new GetResultByMembershipNoQuery(membershipNumber));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpPut("{membershipNumber}")]
        public async Task<IActionResult> UpdateResultAsync([FromRoute] string membershipNumber, UpdateResultRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateResultCommand(membershipNumber, updateRequest));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{membershipNumber}")]
        public async Task<IActionResult> DeleteResultAsync([FromRoute] string membershipNumber)
        {
            var result = await sender.Send(new DeleteResultCommand(membershipNumber));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }
    }
}

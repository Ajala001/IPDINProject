using App.Application.Commands.Result;
using App.Application.Queries.Result;
using App.Core.DTOs.Requests.SearchRequestDtos;
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
        [HttpGet("/{membershipNumber}")]
        public async Task<IActionResult> GetAllResultAsync([FromRoute] string membershipNumber)
        {
            var result = await sender.Send(new GetAllResultQuery(membershipNumber));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{resultId}")]
        public async Task<IActionResult> GetResultAsync([FromRoute] Guid resultId)
        {
            var result = await sender.Send(new GetResultByIdQuery(resultId));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchForResultAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchResultQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
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

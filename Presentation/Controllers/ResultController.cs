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
    public class ResultController : ControllerBase
    {
        private readonly ISender sender;
        public ResultController(ISender _sender)
        {
            sender = _sender;
        }
       
        [HttpGet("member/{membershipNumber}")]
        public async Task<IActionResult> GetMemberResultsAsync([FromRoute] string membershipNumber, [FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            string decodedMembershipNumber = Uri.UnescapeDataString(membershipNumber);
            var result = await sender.Send(new GetMemberResultsQuery(decodedMembershipNumber, pageSize, pageNumber));
            if (result.IsSuccessful) return Ok(result);
            return NotFound(result);
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpGet("batch/{batchResultId}")]
        public async Task<IActionResult> GetBatchResultsAsync([FromRoute] Guid batchResultId, [FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetBatchResultQuery(batchResultId, pageSize, pageNumber));
            if (result.IsSuccessful) return Ok(result);
            return NotFound(result);
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{resultId}")]
        public async Task<IActionResult> GetResultAsync([FromRoute] Guid resultId)
        {
            var result = await sender.Send(new GetResultByIdQuery(resultId));
            if (result.IsSuccessful) return Ok(result);
            return NotFound(result);
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
            return NotFound(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{resultId}")]
        public async Task<IActionResult> DeleteResultAsync([FromRoute] Guid resultId)
        {
            var result = await sender.Send(new DeleteResultCommand(resultId));
            if (result.IsSuccessful) return Ok(result);
            return NotFound(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("download/{resultId}")]
        public async Task<IActionResult> GenerateResult([FromRoute] Guid resultId)
        {
            var result = await sender.Send(new DownloadResultQuery(resultId));
            if (!result.IsSuccessful)
            {
                return NotFound(new { error = result.Message });
            }
            return File(result.Data, "application/pdf", "Result.pdf");
        }
    }
}

using App.Application.Commands.BatchResult;
using App.Application.Queries.BatchResult;
using App.Core.DTOs.Requests.SearchRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/batchResults")]
    [ApiController]
    public class BatchResultController(ISender sender) : ControllerBase
    {

        [Authorize(Roles = "Admin")]
        [HttpPost("uploadResult/{examId}")]
        public async Task<ActionResult> UploadBatchResultAsync([FromForm] IFormFile file, [FromRoute] Guid examId)
        {
            var result = await sender.Send(new UploadBatchResultCommand(file, examId));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBatchResultAsync(int pageSize, int pageNumber)
        {
            var result = await sender.Send(new GetAllBatchResultQuery(pageSize, pageNumber));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{batchId}")]
        public async Task<IActionResult> GetBatchResult([FromRoute] Guid batchId)
        {
            var result = await sender.Send(new GetBatchResultByIdQuery(batchId));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchForBatchResultAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchBatchResultQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{batchId}")]
        public async Task<IActionResult> DeleteBatchResultAsync([FromRoute] Guid batchId)
        {
            var result = await sender.Send(new DeleteBatchResultCommand(batchId));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }
    }
}

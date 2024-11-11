using App.Application.Commands.AppApplication;
using App.Application.Queries.AppApplication;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    //[Authorize(Policy = "PaidDuesOnly", Roles = "Admin, Member")]
    [Route("api/applications")]
    [ApiController]
    public class ApplicationController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddApplicationAsync([FromBody] CreateAppApplicationRequestDto request)
        {
            var result = await sender.Send(new AddAppApplicationCommand(request));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllApplicationsAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetAllAppApplicationsQuery(pageSize, pageNumber));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetUserApplicationAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetUserApplicationsQuery(pageSize, pageNumber));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchForApplicationAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchAppApplicationQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpGet("{applicationId}")]
        public async Task<IActionResult> GetApplicationByIdAsync([FromRoute] Guid applicationId)
        {
            var result = await sender.Send(new GetAppApplicationByIdQuery(applicationId));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }


        [HttpPut("{applicationId}")]
        public async Task<IActionResult> UpdateApplicationAsync([FromRoute] Guid applicationId, [FromBody] UpdateAppApplicationRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateAppApplicationCommand(applicationId, updateRequest));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }


        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteApplicationAsync([FromRoute] Guid applicationId)
        {
            var result = await sender.Send(new DeleteAppApplicationCommand(applicationId));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }

        [HttpGet("download-applicationSlip/{applicationId}")]
        public async Task<IActionResult> GenerateApplicationSlip([FromRoute] Guid applicationId)
        {
            var result = await sender.Send(new DownloadApplicationSlipQuery(applicationId));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }

        [HttpPost("accept/{applicationId}")]
        public async Task<IActionResult> AcceptApplication([FromRoute] Guid applicationId)
        {
            var result = await sender.Send(new AcceptApplicationCommand(applicationId));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }

        [HttpPost("reject/{applicationId}")]
        public async Task<IActionResult> RejectApplication([FromRoute] Guid applicationId, [FromBody] RejectionApplicationRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RejectionReason)) 
                return BadRequest(new { message = "Rejection reason is required." });

            var result = await sender.Send(new RejectApplicationCommand(applicationId, request));
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
            return Ok(result);
        }
    }
}

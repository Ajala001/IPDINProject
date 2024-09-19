using App.Application.Commands.AppApplication;
using App.Application.Queries.AppApplication;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Authorize(Policy = "PaidDuesOnly", Roles = "Admin, Member")]
    [Route("api/application")]
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
        public async Task<IActionResult> GetAllApplicationAsync()
        {
            var result = await sender.Send(new GetAllAppApplicationsQuery());
            if (!result.IsSuccessful) return NotFound(new { error = result.Message });
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
    }
}

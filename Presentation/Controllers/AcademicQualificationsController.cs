using App.Application.Commands;
using App.Application.Queries;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicQualificationsController(ISender sender) : ControllerBase
    {
        [HttpPost("")] //the default route
        public async Task<IActionResult> AddAcademicQualificationAsync([FromBody] CreateAcademicQualificationRequestDto request)
        {
            var result = await sender.Send(new AddAcademicQualificationCommand(request));
            return Ok(result);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllAcademicQualificationAsync()
        {
            var result = await sender.Send(new GetAllAcademicQualificationQuery());
            return Ok(result);
        }

        [HttpGet("{qualificationId}")]
        public async Task<IActionResult> GetAcademicQualificationByIdAsync([FromRoute] Guid qualificationId)
        {
            var result = await sender.Send(new GetAcademicQualificationByIdQuery(qualificationId));
            return Ok(result);
        }

        [HttpPut("{qualificationId}")]
        public async Task<IActionResult> UpdateAcademicQualificationAsync([FromRoute] Guid qualificationId, [FromBody] UpdateAcademicQualificationRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateAcademicQualificationCommand(qualificationId, updateRequest));
            return Ok(result);
        }

        [HttpDelete("{qualificationId}")]
        public async Task<IActionResult> DeleteAcademicQualificationAsync([FromRoute] Guid qualificationId)
        {
            var result = await sender.Send(new DeleteAcademicQualificationCommand(qualificationId));
            return Ok(result);
        }
    }
}

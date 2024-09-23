using App.Application.Commands.AcademicQualification;
using App.Application.Queries.AcademicQualification;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/qualifications")]
    [ApiController]
    public class QualificationsController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddAcademicQualificationAsync([FromBody] CreateAcademicQualificationRequestDto request)
        {
            var result = await sender.Send(new AddAcademicQualificationCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        public async Task<IActionResult> GetAllAcademicQualificationAsync()
        {
            var result = await sender.Send(new GetAllAcademicQualificationQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        
        [HttpGet("{qualificationId}")]
        public async Task<IActionResult> GetAcademicQualificationByIdAsync([FromRoute] Guid qualificationId)
        {
            var result = await sender.Send(new GetAcademicQualificationByIdQuery(qualificationId));
            if (!result.IsSuccessful) NotFound(result);
            return Ok(result);
        }

        [HttpPut("{qualificationId}")]
        public async Task<IActionResult> UpdateAcademicQualificationAsync([FromRoute] Guid qualificationId, [FromBody] UpdateAcademicQualificationRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateAcademicQualificationCommand(qualificationId, updateRequest));
            if (!result.IsSuccessful) NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{qualificationId}")]
        public async Task<IActionResult> DeleteAcademicQualificationAsync([FromRoute] Guid qualificationId)
        {
            var result = await sender.Send(new DeleteAcademicQualificationCommand(qualificationId));
            if (!result.IsSuccessful) NotFound(result);
            return Ok(result);
        }
    }
}

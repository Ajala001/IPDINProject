using App.Application.Commands.RegistrationType;
using App.Application.Queries.RegistrationType;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/registrationType")]
    [ApiController]
    public class RegistrationTypeController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRegistrationTypeAsync([FromBody] CreateRegistrationTypeRequestDto request)
        {
            var result = await sender.Send(new AddRegistrationTypeCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllRegistrationTypeAsync()
        {
            var result = await sender.Send(new GetAllRegistrationTypeQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpGet("{typeId}")]
        public async Task<IActionResult> GetRegistrationByIdAsync([FromRoute] Guid typeId)
        {
            var result = await sender.Send(new GetRegistrationTypeByIdQuery(typeId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{typeId}")]
        public async Task<IActionResult> UpdateCourseAsync([FromRoute] Guid typeId, [FromBody] UpdateRegistrationTypeRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateRegistrationTypeCommand(typeId, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{typeId}")]
        public async Task<IActionResult> DeleteCourseAsync([FromRoute] Guid typeId)
        {
            var result = await sender.Send(new DeleteRegistrationTypeCommand(typeId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

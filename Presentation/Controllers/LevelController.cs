using App.Application.Commands.RegistrationType;
using App.Application.Queries.RegistrationType;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/levels")]
    [ApiController]
    public class LevelController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddLevelAsync([FromBody] CreateLevelRequestDto request)
        {
            var result = await sender.Send(new AddLevelCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllLevelsAsync()
        {
            var result = await sender.Send(new GetAllLevelsQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpGet("{levelId}")]
        public async Task<IActionResult> GetLevelByIdAsync([FromRoute] Guid levelId)
        {
            var result = await sender.Send(new GetLevelByIdQuery(levelId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{levelId}")]
        public async Task<IActionResult> UpdateLevelAsync([FromRoute] Guid levelId, [FromBody] UpdateLevelRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateLevelCommand(levelId, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{levelId}")]
        public async Task<IActionResult> DeleteLevelAsync([FromRoute] Guid levelId)
        {
            var result = await sender.Send(new DeleteLevelComand(levelId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

using App.Application.Commands.Training;
using App.Application.Queries.Training;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/trainings")]
    [ApiController]
    public class TrainingController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddTrainingAsync([FromBody] CreateTrainingRequestDto request)
        {
            var result = await sender.Send(new AddTrainingCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        public async Task<IActionResult> GetAllTrainingAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetAllTrainingQuery(pageSize, pageNumber));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserTrainingsAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetUserTrainingsQuery(pageSize, pageNumber));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchForTrainingAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchTrainingQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{trainingId}")]
        public async Task<IActionResult> GetTrainingByIdAsync([FromRoute] Guid trainingId)
        {
            var result = await sender.Send(new GetTrainingByIdQuery(trainingId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{trainingId}")]
        public async Task<IActionResult> UpdateTrainingAsync([FromRoute] Guid trainingId, [FromBody] UpdateTrainingRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateTrainingCommand(trainingId, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{trainingId}")]
        public async Task<IActionResult> DeleteTrainingAsync([FromRoute] Guid trainingId)
        {
            var result = await sender.Send(new DeleteTrainingCommand(trainingId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

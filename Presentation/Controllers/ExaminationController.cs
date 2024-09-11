using App.Application.Commands.Examination;
using App.Application.Queries.Examination;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/examination")]
    [ApiController]
    public class ExaminationController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddExaminationAsync([FromBody] CreateExaminationRequestDto request)
        {
            var result = await sender.Send(new AddExaminationCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        public async Task<IActionResult> GetAllExaminationAsync()
        {
            var result = await sender.Send(new GetAllExamQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [Authorize(Roles = "Admin, Member")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchForExaminationAsync([FromQuery] ExaminationSearchRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchExamQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{examId}")]
        public async Task<IActionResult> GetExaminationByIdAsync([FromRoute] Guid examId)
        {
            var result = await sender.Send(new GetExamByIdQuery(examId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{examId}")]
        public async Task<IActionResult> UpdateExamintionAsync([FromRoute] Guid examId, [FromBody] UpdateExaminationRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateExaminationCommand(examId, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{examId}")]
        public async Task<IActionResult> DeleteExaminationAsync([FromRoute] Guid examId)
        {
            var result = await sender.Send(new DeleteExaminationCommand(examId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

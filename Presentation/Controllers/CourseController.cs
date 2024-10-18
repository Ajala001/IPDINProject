using App.Application.Commands.Course;
using App.Application.Queries.Course;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCourseAsync([FromBody] CreateCourseRequestDto request)
        {
            var result = await sender.Send(new AddCourseCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        public async Task<IActionResult> GetAllCourseAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetAllCourseQuery(pageSize, pageNumber));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchForCourseAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchCourseQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Member")]
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseByIdAsync([FromRoute] Guid courseId)
        {
            var result = await sender.Send(new GetCourseByIdQuery(courseId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourseAsync([FromRoute] Guid courseId, [FromBody] UpdateCourseRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateCourseCommand(courseId, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourseAsync([FromRoute] Guid courseId)
        {
            var result = await sender.Send(new DeleteCourseCommand(courseId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

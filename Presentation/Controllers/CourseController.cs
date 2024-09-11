using App.Application.Commands.Course;
using App.Application.Queries.Course;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Route("api/course")]
    [ApiController]
    public class CourseController(ISender sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddCourseAsync([FromBody] CreateCourseRequestDto request)
        {
            var result = await sender.Send(new AddCourseCommand(request));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCourseAsync()
        {
            var result = await sender.Send(new GetAllCourseQuery());
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchForCourseAsync([FromQuery] CourseSearchRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchCourseQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseByIdAsync([FromRoute] Guid courseId)
        {
            var result = await sender.Send(new GetCourseByIdQuery(courseId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourseAsync([FromRoute] Guid courseId, [FromBody] UpdateCourseRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdateCourseCommand(courseId, updateRequest));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }


        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourseAsync([FromRoute] Guid courseId)
        {
            var result = await sender.Send(new DeleteCourseCommand(courseId));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }
    }
}

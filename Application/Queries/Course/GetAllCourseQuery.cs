using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Course
{
    public record GetAllCourseQuery() : IRequest<ApiResponse<IEnumerable<CourseResponseDto>>>;

    public class GetAllCourseQueryHandler(ICourseService courseService)
        : IRequestHandler<GetAllCourseQuery, ApiResponse<IEnumerable<CourseResponseDto>>>
{
        public async Task<ApiResponse<IEnumerable<CourseResponseDto>>> Handle(GetAllCourseQuery request, CancellationToken cancellationToken)
        {
            return await courseService.GetCoursesAsync();
        }
    }
}

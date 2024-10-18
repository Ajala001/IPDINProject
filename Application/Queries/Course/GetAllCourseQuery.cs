using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Course
{
    public record GetAllCourseQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<CourseResponseDto>>>;

    public class GetAllCourseQueryHandler(ICourseService courseService)
        : IRequestHandler<GetAllCourseQuery, PagedResponse<IEnumerable<CourseResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<CourseResponseDto>>> Handle(GetAllCourseQuery request, CancellationToken cancellationToken)
        {
            return await courseService.GetCoursesAsync(request.PageSize, request.PageNumber);
        }
    }
}

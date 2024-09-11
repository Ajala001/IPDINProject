using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Course
{
    public record GetCourseByIdQuery(Guid CourseId) : IRequest<ApiResponse<CourseResponseDto>>;

    public class GetCourseByIdQueryHandler(ICourseService courseService)
        : IRequestHandler<GetCourseByIdQuery, ApiResponse<CourseResponseDto>>
    {
        public async Task<ApiResponse<CourseResponseDto>> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            return await courseService.GetCourseAsync(request.CourseId);
        }
    }
}

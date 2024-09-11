using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Course
{
    public record SearchCourseQuery(CourseSearchRequestDto SearchRequestDto) 
        : IRequest<ApiResponse<IEnumerable<CourseResponseDto>>>;

    public class SearchCourseQueryHandler(ICourseService courseService)
        : IRequestHandler<SearchCourseQuery, ApiResponse<IEnumerable<CourseResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<CourseResponseDto>>> Handle(SearchCourseQuery request, CancellationToken cancellationToken)
        {
            return await courseService.SearchCourseAsync(request.SearchRequestDto);
        }
    }
}

using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Course
{
    public record SearchCourseQuery(SearchQueryRequestDto SearchRequestDto) 
        : IRequest<PagedResponse<IEnumerable<CourseResponseDto>>>;

    public class SearchCourseQueryHandler(ICourseService courseService)
        : IRequestHandler<SearchCourseQuery, PagedResponse<IEnumerable<CourseResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<CourseResponseDto>>> Handle(SearchCourseQuery request, CancellationToken cancellationToken)
        {
            return await courseService.SearchCourseAsync(request.SearchRequestDto);
        }
    }
}

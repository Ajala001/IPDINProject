using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Course
{
    public record AddCourseCommand(CreateCourseRequestDto Course) : IRequest<ApiResponse<CourseResponseDto>>;

    public class AddCourseCommandHandler(ICourseService courseService)
        : IRequestHandler<AddCourseCommand, ApiResponse<CourseResponseDto>>
    {
        public async Task<ApiResponse<CourseResponseDto>> Handle(AddCourseCommand request, CancellationToken cancellationToken)
        {
            return await courseService.CreateAsync(request.Course);
        }
    }
}

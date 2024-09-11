using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Course
{
    public record UpdateCourseCommand(Guid CourseId, UpdateCourseRequestDto UpdateRequest) 
        : IRequest<ApiResponse<CourseResponseDto>>;

    public class UpdateCourseCommandHandler(ICourseService courseService)
        : IRequestHandler<UpdateCourseCommand, ApiResponse<CourseResponseDto>>
    {
        public async Task<ApiResponse<CourseResponseDto>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            return await courseService.UpdateAsync(request.CourseId, request.UpdateRequest);
        }
    }
}

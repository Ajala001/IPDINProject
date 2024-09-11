using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Course
{
    public record DeleteCourseCommand(Guid CourseId) : IRequest<ApiResponse<CourseResponseDto>>;

    public class DeleteCourseCommandHandler(ICourseService courseService)
        : IRequestHandler<DeleteCourseCommand, ApiResponse<CourseResponseDto>>
    {
        public async Task<ApiResponse<CourseResponseDto>> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            return await courseService.DeleteAsync(request.CourseId);
        }
    }
}

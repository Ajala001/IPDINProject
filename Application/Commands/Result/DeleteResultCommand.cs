using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Result
{
    public record DeleteResultCommand(Guid ResultId) : IRequest<ApiResponse<StudentResultResponseDto>>;

    public class DeleteResultCommandHandler(IResultService resultService)
        : IRequestHandler<DeleteResultCommand, ApiResponse<StudentResultResponseDto>>
    {
        public async Task<ApiResponse<StudentResultResponseDto>> Handle(DeleteResultCommand request, CancellationToken cancellationToken)
        {
            return await resultService.DeleteAsync(request.ResultId);
        }
    }
}

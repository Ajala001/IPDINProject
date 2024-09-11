using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Result
{
    public record DeleteResultCommand(string MembershipNumber) : IRequest<ApiResponse<ResultResponseDto>>;

    public class DeleteResultCommandHandler(IResultService resultService)
        : IRequestHandler<DeleteResultCommand, ApiResponse<ResultResponseDto>>
    {
        public async Task<ApiResponse<ResultResponseDto>> Handle(DeleteResultCommand request, CancellationToken cancellationToken)
        {
            return await resultService.DeleteAsync(request.MembershipNumber);
        }
    }
}

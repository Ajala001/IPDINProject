using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.BatchResult
{
    public record DeleteBatchResultCommand(Guid BathcId) : IRequest<ApiResponse<BulkResultResponseDto>>;

    public class DeleteBatchResultCommandHandler(IBatchResultService batchResultService)
        : IRequestHandler<DeleteBatchResultCommand, ApiResponse<BulkResultResponseDto>>
    {
        public async Task<ApiResponse<BulkResultResponseDto>> Handle(DeleteBatchResultCommand request, CancellationToken cancellationToken)
        {
            return await batchResultService.DeleteAsync(request.BathcId);
        }
    }
}

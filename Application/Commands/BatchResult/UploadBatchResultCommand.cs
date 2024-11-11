using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace App.Application.Commands.BatchResult
{
    public record UploadBatchResultCommand(IFormFile File, Guid ExamId) : IRequest<ApiResponse<BulkResultResponseDto>>;

    public class UploadResultCommandHandler(IBatchResultService batchResultService)
        : IRequestHandler<UploadBatchResultCommand, ApiResponse<BulkResultResponseDto>>
    {
        public async Task<ApiResponse<BulkResultResponseDto>> Handle(UploadBatchResultCommand request, CancellationToken cancellationToken)
        {
            return await batchResultService.UploadBatchResultAsync(request.File, request.ExamId);
        }
    }
}

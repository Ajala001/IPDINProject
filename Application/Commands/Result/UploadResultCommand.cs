using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace App.Application.Commands.Result
{
    public record UploadResultCommand(IFormFile File) : IRequest<ApiResponse<IEnumerable<ResultResponseDto>>>;

    public class UploadResultCommandHandler(IResultService resultService)
        : IRequestHandler<UploadResultCommand, ApiResponse<IEnumerable<ResultResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<ResultResponseDto>>> Handle(UploadResultCommand request, CancellationToken cancellationToken)
        {
            return await resultService.UploadResultAsync(request.File);
        }
    }
}

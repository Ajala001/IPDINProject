using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record GetResultByIdQuery(Guid ResultId) : IRequest<ApiResponse<StudentResultResponseDto>>;

    public class GetResultByMembershipNoQueryHandler(IResultService resultService)
        : IRequestHandler<GetResultByIdQuery, ApiResponse<StudentResultResponseDto>>
    {
        public async Task<ApiResponse<StudentResultResponseDto>> Handle(GetResultByIdQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GetResultAsync(request.ResultId);
        }
    }
}

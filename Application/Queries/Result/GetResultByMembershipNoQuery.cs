using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record GetResultByMembershipNoQuery(string MembershipNo) : IRequest<ApiResponse<ResultResponseDto>>;

    public class GetResultByMembershipNoQueryHandler(IResultService resultService)
        : IRequestHandler<GetResultByMembershipNoQuery, ApiResponse<ResultResponseDto>>
    {
        public async Task<ApiResponse<ResultResponseDto>> Handle(GetResultByMembershipNoQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GetResultAsync(request.MembershipNo);
        }
    }
}

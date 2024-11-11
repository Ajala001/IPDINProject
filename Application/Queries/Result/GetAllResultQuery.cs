using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record GetAllResultQuery(string MembershipNumber) : IRequest<ApiResponse<IEnumerable<StudentResultResponseDto>>>;

    public class GetAllResultsQueryHandler(IResultService resultService)
        : IRequestHandler<GetAllResultQuery, ApiResponse<IEnumerable<StudentResultResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<StudentResultResponseDto>>> Handle(GetAllResultQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GetResultsAsync(request.MembershipNumber);
        }
    }
}

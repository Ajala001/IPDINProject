using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record GetAllResultQuery() : IRequest<ApiResponse<IEnumerable<ResultResponseDto>>>;

    public class GetAllResultQueryHandler(IResultService resultService)
        : IRequestHandler<GetAllResultQuery, ApiResponse<IEnumerable<ResultResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<ResultResponseDto>>> Handle(GetAllResultQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GetResultsAsync();
        }
    }
}

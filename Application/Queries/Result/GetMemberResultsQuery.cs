using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record GetMemberResultsQuery(string MembershipNumber, int PageSize, int PageNumber) 
        : IRequest<PagedResponse<IEnumerable<StudentResultResponseDto>>>;

    public class GetMemberResultsQueryHandler(IResultService resultService)
        : IRequestHandler<GetMemberResultsQuery, PagedResponse<IEnumerable<StudentResultResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> Handle(GetMemberResultsQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GetMemberResultsAsync(request.MembershipNumber, request.PageSize, request.PageNumber);
        }
    }
}

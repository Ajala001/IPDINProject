using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Result
{
    public record UpdateResultCommand(string MembershipNumber, UpdateResultRequestDto UpdateRequest) 
        : IRequest<ApiResponse<StudentResultResponseDto>>;

    public class UpdateResultCommandHandler(IResultService resultService)
        : IRequestHandler<UpdateResultCommand, ApiResponse<StudentResultResponseDto>>
    {
        public async Task<ApiResponse<StudentResultResponseDto>> Handle(UpdateResultCommand request, CancellationToken cancellationToken)
        {
            return await resultService.UpdateAsync(request.MembershipNumber, request.UpdateRequest);
        }
    }
}

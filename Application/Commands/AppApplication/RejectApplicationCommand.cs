using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.AppApplication
{
    public record RejectApplicationCommand(Guid ApplicationId, RejectionApplicationRequestDto RejectionRequest)
        : IRequest<ApiResponse<string>>;

    public class RejectApplicationCommandHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<RejectApplicationCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
        {
            return await appApplicationService.RejectApplicationAsync(request.ApplicationId, request.RejectionRequest);
        }
    }
}

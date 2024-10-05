using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.AppApplication
{
    public record AcceptApplicationCommand(Guid ApplicationId) : IRequest<ApiResponse<AppApplicationResponseDto>>;

    public class AcceptApplicationCommandHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<AcceptApplicationCommand, ApiResponse<AppApplicationResponseDto>>
    {
        public async Task<ApiResponse<AppApplicationResponseDto>> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
        {
            return await appApplicationService.AcceptApplicationAsync(request.ApplicationId);
        }
    }
}

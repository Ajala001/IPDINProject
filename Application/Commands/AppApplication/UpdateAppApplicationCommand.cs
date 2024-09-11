using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.AppApplication
{
    public record UpdateAppApplicationCommand(Guid ApplicationId, UpdateAppApplicationRequestDto UpdateRequest) 
        : IRequest<ApiResponse<AppApplicationResponseDto>>;

    public class UpdateAppApplicationCommandHandler(IAppApplicationService appApplicationService) :
        IRequestHandler<UpdateAppApplicationCommand, ApiResponse<AppApplicationResponseDto>>
    {
        public async Task<ApiResponse<AppApplicationResponseDto>> Handle(UpdateAppApplicationCommand request, CancellationToken cancellationToken)
        {
            return await appApplicationService.UpdateAsync(request.ApplicationId, request.UpdateRequest);
        }
    }
}

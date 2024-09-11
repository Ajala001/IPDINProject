using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.AppApplication
{
    public record AddAppApplicationCommand(CreateAppApplicationRequestDto AppApplication)
        : IRequest<ApiResponse<AppApplicationResponseDto>>;

    public class AddAppApplicationCommandHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<AddAppApplicationCommand, ApiResponse<AppApplicationResponseDto>>
    {
        public async Task<ApiResponse<AppApplicationResponseDto>> Handle(AddAppApplicationCommand request, CancellationToken cancellationToken)
        {
            return await appApplicationService.CreateAsync(request.AppApplication);
        }
    }
}

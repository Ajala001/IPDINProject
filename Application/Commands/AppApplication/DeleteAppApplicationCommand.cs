using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.AppApplication
{
    public record DeleteAppApplicationCommand(Guid ApplicationId) : IRequest<ApiResponse<AppApplicationResponseDto>>;

    public class DeleteAppApplicationCommandHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<DeleteAppApplicationCommand, ApiResponse<AppApplicationResponseDto>>
    {
        public async Task<ApiResponse<AppApplicationResponseDto>> Handle(DeleteAppApplicationCommand request, CancellationToken cancellationToken)
        {
            return await appApplicationService.DeleteAsync(request.ApplicationId); 
        }
    }
}

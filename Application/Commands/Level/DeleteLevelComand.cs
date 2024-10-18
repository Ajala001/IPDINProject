using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.RegistrationType
{
    public record DeleteLevelComand(Guid LevelId)
      : IRequest<ApiResponse<LevelRequestResponseDto>>;


    public class DeleteRegistrationTypeCommandHandler(ILevelService levelService)
    : IRequestHandler<DeleteLevelComand, ApiResponse<LevelRequestResponseDto>>
    {
        public async Task<ApiResponse<LevelRequestResponseDto>> Handle(DeleteLevelComand request, CancellationToken cancellationToken)
        {
            return await levelService.DeleteAsync(request.LevelId);
        }
    }
}

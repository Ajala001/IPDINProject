using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.RegistrationType
{
    public record UpdateLevelCommand(Guid LevelId, UpdateLevelRequestDto UpdateRequest)
      : IRequest<ApiResponse<LevelRequestResponseDto>>;


    public class UpdateRegistrationTypeCommandHandler(ILevelService levelService)
    : IRequestHandler<UpdateLevelCommand, ApiResponse<LevelRequestResponseDto>>
    {
        public async Task<ApiResponse<LevelRequestResponseDto>> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
        {
            return await levelService.UpdateAsync(request.LevelId, request.UpdateRequest);
        }
    }
}

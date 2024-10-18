using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.RegistrationType
{
    public record AddLevelCommand(CreateLevelRequestDto level) 
        : IRequest<ApiResponse<LevelRequestResponseDto>>;


    public class AddRegistrationTypeCommandHandler(ILevelService levelService)
    : IRequestHandler<AddLevelCommand, ApiResponse<LevelRequestResponseDto>>
    {
        public async Task<ApiResponse<LevelRequestResponseDto>> Handle(AddLevelCommand request, CancellationToken cancellationToken)
        {
            return await levelService.CreateAsync(request.level);
        }
    }
}


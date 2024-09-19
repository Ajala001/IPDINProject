using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.RegistrationType
{
    public record UpdateRegistrationTypeCommand(Guid TypeId, UpdateRegistrationTypeRequestDto UpdateRequest)
      : IRequest<ApiResponse<RegistrationTypeResponseDto>>;


    public class UpdateRegistrationTypeCommandHandler(IRegistrationTypeService typeService)
    : IRequestHandler<UpdateRegistrationTypeCommand, ApiResponse<RegistrationTypeResponseDto>>
    {
        public async Task<ApiResponse<RegistrationTypeResponseDto>> Handle(UpdateRegistrationTypeCommand request, CancellationToken cancellationToken)
        {
            return await typeService.UpdateAsync(request.TypeId, request.UpdateRequest);
        }
    }
}

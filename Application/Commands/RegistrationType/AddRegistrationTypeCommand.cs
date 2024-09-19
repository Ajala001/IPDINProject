using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.RegistrationType
{
    public record AddRegistrationTypeCommand(CreateRegistrationTypeRequestDto Type) 
        : IRequest<ApiResponse<RegistrationTypeResponseDto>>;


    public class AddRegistrationTypeCommandHandler(IRegistrationTypeService typeService)
    : IRequestHandler<AddRegistrationTypeCommand, ApiResponse<RegistrationTypeResponseDto>>
    {
        public async Task<ApiResponse<RegistrationTypeResponseDto>> Handle(AddRegistrationTypeCommand request, CancellationToken cancellationToken)
        {
            return await typeService.CreateAsync(request.Type);
        }
    }
}


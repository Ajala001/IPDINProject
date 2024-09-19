using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.RegistrationType
{
    public record DeleteRegistrationTypeCommand(Guid TypeId)
      : IRequest<ApiResponse<RegistrationTypeResponseDto>>;


    public class DeleteRegistrationTypeCommandHandler(IRegistrationTypeService typeService)
    : IRequestHandler<DeleteRegistrationTypeCommand, ApiResponse<RegistrationTypeResponseDto>>
    {
        public async Task<ApiResponse<RegistrationTypeResponseDto>> Handle(DeleteRegistrationTypeCommand request, CancellationToken cancellationToken)
        {
            return await typeService.DeleteAsync(request.TypeId);
        }
    }
}

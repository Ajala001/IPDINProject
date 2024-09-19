using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.RegistrationType
{
    public record GetAllRegistrationTypeQuery() : IRequest<ApiResponse<IEnumerable<RegistrationTypeResponseDto>>>;

    public class GetAllRegistrationTypeQueryHandler(IRegistrationTypeService registrationTypeService)
        : IRequestHandler<GetAllRegistrationTypeQuery, ApiResponse<IEnumerable<RegistrationTypeResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<RegistrationTypeResponseDto>>> Handle(GetAllRegistrationTypeQuery request, CancellationToken cancellationToken)
        {
            return await registrationTypeService.GetRegistrationTypesAsync();
        }
    }
}

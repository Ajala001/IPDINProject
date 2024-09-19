using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Queries.RegistrationType
{
    public record GetRegistrationTypeByIdQuery(Guid TypeId) : IRequest<ApiResponse<RegistrationTypeResponseDto>>;

    public class GetRegistrationTypeByIdQueryHandler(IRegistrationTypeService registrationTypeService)
        : IRequestHandler<GetRegistrationTypeByIdQuery, ApiResponse<RegistrationTypeResponseDto>>
{
        public async Task<ApiResponse<RegistrationTypeResponseDto>> Handle(GetRegistrationTypeByIdQuery request, CancellationToken cancellationToken)
        {
            return await registrationTypeService.GetGetRegistrationTypeAsync(request.TypeId);
        }
    }
}

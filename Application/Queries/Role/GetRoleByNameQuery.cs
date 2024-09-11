using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Role
{
    public record GetRoleByNameQuery(string RoleName) : IRequest<ApiResponse<RoleResponseDto>>;

    public class GetRoleByNameQueryHandler(IRoleService roleService)
        : IRequestHandler<GetRoleByNameQuery, ApiResponse<RoleResponseDto>>
    {
        public async Task<ApiResponse<RoleResponseDto>> Handle(GetRoleByNameQuery request, CancellationToken cancellationToken)
        {
           return await roleService.GetRoleAsync(request.RoleName);
        }
    }
}

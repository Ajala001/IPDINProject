using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Role
{
    public record GetAllRoleQuery() : IRequest<ApiResponse<IEnumerable<RoleResponseDto>>>;

    public class GetAllRoleQueryHandler(IRoleService roleService) : IRequestHandler<GetAllRoleQuery, ApiResponse<IEnumerable<RoleResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<RoleResponseDto>>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            return await roleService.GetRolesAsync();
        }
    }
}

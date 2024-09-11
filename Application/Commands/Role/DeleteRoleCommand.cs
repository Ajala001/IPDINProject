using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Role
{
    public record DeleteRoleCommand(string RoleName) : IRequest<ApiResponse<RoleResponseDto>>;

    public class DeleteRoleCommandHandler(IRoleService roleService)
        : IRequestHandler<DeleteRoleCommand, ApiResponse<RoleResponseDto>>
    {
        public async Task<ApiResponse<RoleResponseDto>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.DeleteAsync(request.RoleName);
        }
    }
}

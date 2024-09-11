using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Role
{
    public record UpdateRoleCommand(string RoleName, UpdateRoleRequestDto UpdateRequest)
        : IRequest<ApiResponse<RoleResponseDto>>;

    public class UpdateRoleCommandHandler(IRoleService roleService)
        : IRequestHandler<UpdateRoleCommand, ApiResponse<RoleResponseDto>>
    {
        public async Task<ApiResponse<RoleResponseDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.UpdateAsync(request.RoleName, request.UpdateRequest);
        }
    }
}
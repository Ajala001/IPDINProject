using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Role
{
    public record AddRoleCommand(CreateRoleRequestDto Role) : IRequest<ApiResponse<RoleResponseDto>>;

    public class AddRoleCommandHandler(IRoleService roleService)
        : IRequestHandler<AddRoleCommand, ApiResponse<RoleResponseDto>>
    {
        public async Task<ApiResponse<RoleResponseDto>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            return await roleService.CreateAsync(request.Role);
        }
    }
}

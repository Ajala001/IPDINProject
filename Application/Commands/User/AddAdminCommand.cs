using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.User
{
    public record AddAdminCommand(AddAdminDto AddAdminDto)
       : IRequest<ApiResponse<UserResponseDto>>;

    public class AddAdminCommandHandler(IUserService userService)
        : IRequestHandler<AddAdminCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(AddAdminCommand request, CancellationToken cancellationToken)
        {
            return await userService.AddAdminAsync(request.AddAdminDto);
        }
    }
}

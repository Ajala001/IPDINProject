using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Queries.User
{
    public record GetAllUserQuery() : IRequest<ApiResponse<IEnumerable<UserResponseDto>>>;

    public class GetAllUserQueryHandler(IUserService userService)
        : IRequestHandler<GetAllUserQuery, ApiResponse<IEnumerable<UserResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<UserResponseDto>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await userService.GetUsersAsync();
        }
    }
}

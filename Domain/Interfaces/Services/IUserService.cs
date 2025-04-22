using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponseDto>> AddAdminAsync(AddAdminDto addAdminDto);
        Task<ApiResponse<UserResponseDto>> GetUserAsync(string email);
        Task<PagedResponse<IEnumerable<UserResponseDto>>> GetUsersAsync(int pageSize, int pageNumber);
        Task<ApiResponse<UserResponseDto>> DeleteAsync(string email);
        Task<ApiResponse<UserResponseDto>> UpdateAsync(string email, UpdateUserRequestDto request);
        Task<PagedResponse<IEnumerable<UserResponseDto>>> SearchUserAsync(SearchQueryRequestDto request);
    }
}

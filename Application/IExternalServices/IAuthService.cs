using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Application.IExternalServices
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> SignInAsync(SignInRequestDto request);
        Task SignOutAsync();
        Task<ApiResponse<UserResponseDto>> SignUpAsync(SignUpRequestDto request);
        Task<ApiResponse<UserResponseDto>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<UserResponseDto>> ForgetPasswordAsync(string email);
        Task<ApiResponse<UserResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}

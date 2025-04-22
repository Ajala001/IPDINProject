using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;

namespace App.Application.IExternalServices
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> SignInAsync(SignInRequestDto request);
        Task<ApiResponse<string>> SignOutAsync();
        Task<ApiResponse<UserResponseDto>> SignUpAsync(SignUpRequestDto request);
        Task<ApiResponse<UserResponseDto>> ConfirmEmailAsync(string email, string token);
        Task<ApiResponse<UserResponseDto>> ForgetPasswordAsync(string email);
        Task<ApiResponse<UserResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse<UserResponseDto>> ChangePasswordAsync(ChangePasswordRequestDto request);
        Task<ApiResponse<string>> ResendEmailConfirmationToken(string email);
        Task<User?> AuthenticateUserAsync(string? token);
    }
}

using App.Core.Entities;
using App.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        string GeneratePaymentToken(string referenceNo);
        bool ValidateToken(string token, out string refNo);
        string GeneratePaymentToken(Guid userId, Guid serviceId, PaymentType paymentType);
        (bool IsValid, string? Email) ValidateUserToken(string token);
    }
}

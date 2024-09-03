using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Infrastructure.ExternalServices.Payments
{
    public interface IPaymentService
    {
        Task<string> InitiatePayment(CreatePaymentRequestDto request);
        Task<string> VerifyPayment(string referenceNo);
        Task<ApiResponse<PaymentResponseDto>> UpdateAsync(string referenceNo, UpdatePaymentRequestDto request);
        Task<ApiResponse<PaymentResponseDto>> DeleteAsync(string referenceNo);
        Task<ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsAsync();
        Task<ApiResponse<PaymentResponseDto>> GetPayment(string referenceNo);
    }
}
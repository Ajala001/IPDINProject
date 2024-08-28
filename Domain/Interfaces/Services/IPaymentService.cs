using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponseDto>> InitiatePayment(CreatePaymentRequestDto request);
        Task<ApiResponse<PaymentResponseDto>> UpdateAsync(Payment payment);
        Task<ApiResponse<PaymentResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsAsync();
        Task<ApiResponse<PaymentResponseDto>> GetPayment(Guid referenceNo);
    }
}
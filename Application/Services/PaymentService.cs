using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Services;

namespace App.Application.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<ApiResponse<PaymentResponseDto>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PaymentResponseDto>> GetPayment(Guid referenceNo)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PaymentResponseDto>> InitiatePayment(CreatePaymentRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<PaymentResponseDto>> UpdateAsync(Payment payment)
        {
            throw new NotImplementedException();
        }
    }
}

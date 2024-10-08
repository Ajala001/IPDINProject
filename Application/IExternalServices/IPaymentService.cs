﻿using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Application.IExternalServices
{
    public interface IPaymentService
    {
        Task<ApiResponse<string>> InitiatePaymentAsync(CreatePaymentRequestDto request);
        Task<ApiResponse<string>> VerifyPaymentAsync(string referenceNo);
        Task<ApiResponse<PaymentResponseDto>> UpdateAsync(string referenceNo, UpdatePaymentRequestDto request);
        Task<ApiResponse<PaymentResponseDto>> DeleteAsync(string referenceNo);
        Task<ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsAsync();
        Task<ApiResponse<PaymentResponseDto>> GetPaymentAsync(string referenceNo);
    }
}
using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PayStack.Net;
using System.Security.Claims;

namespace App.Infrastructure.ExternalServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IExaminationRepository _examinationRepository;
        private readonly ITrainingRepository _trainingRepository;
        private readonly UserManager<User> _userManager;
        private readonly string token;

        private PayStackApi PayStack { get; set; }
        public PaymentService(IHttpContextAccessor httpContextAccessor, IPaymentRepository paymentRepository,
            IConfiguration configuration, IUnitOfWork unitOfWork, IExaminationRepository examinationRepository,
            ITrainingRepository trainingRepository, UserManager<User> userManager)
        {
            _contextAccessor = httpContextAccessor;
            _paymentRepository = paymentRepository;
            _trainingRepository = trainingRepository;
            _examinationRepository = examinationRepository;
            _configuration = configuration;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            token = _configuration["Payment:PaystackSK"]!;
            PayStack = new PayStackApi(token);

        }

        public async Task<Core.DTOs.Responses.ApiResponse<PaymentResponseDto>> DeleteAsync(string referenceNo)
        {
            var payment = await _paymentRepository.GetPaymentAsync(p => p.PaymentRef == referenceNo);
            if (payment == null) return new Core.DTOs.Responses.ApiResponse<PaymentResponseDto>
            {
                IsSuccessful = false,
                Message = "Payment not found",
                Data = null
            };

            _paymentRepository.Delete(payment);
            await _unitOfWork.SaveAsync();

            return new Core.DTOs.Responses.ApiResponse<PaymentResponseDto>
            {
                IsSuccessful = true,
                Message = "Payment Deleted Successfully",
                Data = null
            };
        }

        public async Task<Core.DTOs.Responses.ApiResponse<PaymentResponseDto>> GetPaymentAsync(string referenceNo)
        {
            var payment = await _paymentRepository.GetPaymentAsync(p => p.PaymentRef == referenceNo);
            if (payment == null) return new Core.DTOs.Responses.ApiResponse<PaymentResponseDto>
            {
                IsSuccessful = false,
                Message = "Payment not found",
                Data = null
            };

            return new Core.DTOs.Responses.ApiResponse<PaymentResponseDto>
            {
                IsSuccessful = true,
                Message = "Payment Retrieved Successfully",
                Data = new PaymentResponseDto
                {
                    Id = payment.Id,
                    PayerFullName = $"{payment.User.FirstName} {payment.User.LastName}",
                    Amount = payment.Amount,
                    PaymentRef = payment.PaymentRef,
                    PaymentFor = payment.ReasonForPayment,
                    CreatedAt = payment.CreatedOn,
                    Status = payment.Status
                }
            };
        }

        public async Task<Core.DTOs.Responses.ApiResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsAsync()
        {
            var payments = await _paymentRepository.GetPaymentsAsync();
            if (!payments.Any()) return new Core.DTOs.Responses.ApiResponse<IEnumerable<PaymentResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Payment Received",
            };

            var responseData = payments.Select(payment => new PaymentResponseDto
            {
                Id = payment.Id,
                PayerFullName = $"{payment.User.FirstName} {payment.User.LastName}",
                Amount = payment.Amount,
                PaymentRef = payment.PaymentRef,
                PaymentFor = payment.ReasonForPayment,
                CreatedAt = payment.CreatedOn,
                Status = payment.Status
            }).ToList();

            return new Core.DTOs.Responses.ApiResponse<IEnumerable<PaymentResponseDto>>
            {
                IsSuccessful = true,
                Message = "Payments Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<Core.DTOs.Responses.ApiResponse<string>> InitiatePaymentAsync(CreatePaymentRequestDto requestDto)
        {
            var loginUser = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(loginUser!);
            var response = await GetFeeAndEntityNameAsync(requestDto.ReasonForPayment, requestDto.EntityId);
            TransactionInitializeRequest request = new TransactionInitializeRequest()
            {
                AmountInKobo = response.fee * 100, //to get the kobo value of the amount
                Email = user!.Email,
                Reference = Guid.NewGuid().ToString(),
                Currency = "NGN",
                CallbackUrl = "https://localhost:7237/payment/verify"
            };

            TransactionInitializeResponse payStackResponse = PayStack.Transactions.Initialize(request);
            if (payStackResponse.Status)
            {
                var payment = new Payment
                {
                    UserId = user!.Id,
                    User = user!,
                    Amount = response.fee,
                    PaymentRef = request.Reference,
                    ReasonForPayment = requestDto.ReasonForPayment,
                    CreatedBy = user!.Email,
                    CreatedOn = DateTime.UtcNow
                };

                await _paymentRepository.CreateAsync(payment);
                await _unitOfWork.SaveAsync();
                return new Core.DTOs.Responses.ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = payStackResponse.Message,
                    Data = payStackResponse.Data.AuthorizationUrl
                };
                    
            }
            return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = true,
                Message = payStackResponse.Message
            };
        }

        public async Task<Core.DTOs.Responses.ApiResponse<PaymentResponseDto>> UpdateAsync(string referenceNo, UpdatePaymentRequestDto request)
        {
            var loginUser = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var payment = await _paymentRepository.GetPaymentAsync(p => p.PaymentRef == referenceNo);
            if (payment == null) return new Core.DTOs.Responses.ApiResponse<PaymentResponseDto>
            {
                IsSuccessful = false,
                Message = "Payment not found",
                Data = null
            };

            payment.ReasonForPayment = request.ReasonForPayment ?? payment.ReasonForPayment;
            payment.Status = request.Status ?? payment.Status;
            payment.ModifiedBy = loginUser!;
            payment.ModifiedOn = DateTime.Now;
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveAsync();

            return new Core.DTOs.Responses.ApiResponse<PaymentResponseDto>
            {
                IsSuccessful = true,
                Message = "Payment Updated Successfully",
                Data = new PaymentResponseDto
                {
                    Id = payment.Id,
                    PayerFullName = $"{payment.User.FirstName} {payment.User.LastName}",
                    Amount = payment.Amount,
                    PaymentRef = payment.PaymentRef,
                    PaymentFor = payment.ReasonForPayment,
                    CreatedAt = payment.CreatedOn,
                    Status = payment.Status
                }
            };
        }

        private async Task<(int fee, string entityName)> GetFeeAndEntityNameAsync(string paymentReason, Guid entityId)
        {
            int fee = 0;
            string entityName = string.Empty;

            if (paymentReason == "Training")
            {
                var training = await _trainingRepository.GetTrainingAsync(t => t.Id == entityId)
                    ?? throw new Exception("Training not found");
                fee = training.Fee;
                entityName = training.Title;
            }
            else if (paymentReason == "Exam")
            {
                var exam = await _examinationRepository.GetExaminationAsync(e => e.Id == entityId)
                    ?? throw new Exception("Exam not found");
                fee = exam.Fee;
                entityName = exam.ExamTitle;
            }
            return (fee, entityName);
        }

        public async Task<Core.DTOs.Responses.ApiResponse<string>> VerifyPaymentAsync(string referenceNo)
        {
            TransactionVerifyResponse response = PayStack.Transactions.Verify(referenceNo);
            if (response.Data.Status == "success")
            {
                var payment = await _paymentRepository.GetPaymentAsync(p => p.PaymentRef == referenceNo);
                if (payment != null)
                {
                    payment.Status = Core.Enums.PaymentStatus.Successful;
                    _paymentRepository.Update(payment);
                    await _unitOfWork.SaveAsync();
                    return new Core.DTOs.Responses.ApiResponse<string>
                    {
                        IsSuccessful = true,
                        Message = response.Message
                    };
                }
            }
            return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = true,
                Message = response.Data.GatewayResponse
            };
        }
    }
}

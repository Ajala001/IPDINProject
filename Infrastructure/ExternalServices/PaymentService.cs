using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Repositories;
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

        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> GetPaymentsAsync(int pageSize, int pageNumber)
        {
            var payments = await _paymentRepository.GetPaymentsAsync();
            if (payments == null || !payments.Any())
            {
                return new PagedResponse<IEnumerable<PaymentResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "Payments Not Found",
                    Data = null
                };
            }

            // If pageSize and pageNumber are not provided (null or 0), return all courses without pagination
            if (pageSize == 0 || pageNumber == 0)
            {
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

                return new PagedResponse<IEnumerable<PaymentResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Payments Retrieved Successfully",
                    TotalRecords = payments.Count(),
                    Data = responseData
                };
            }


            var totalRecords = payments.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // If pageNumber exceeds total pages, return an empty response
            if (pageNumber > totalPages)
            {
                return new PagedResponse<IEnumerable<PaymentResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "No more payments available",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = new List<PaymentResponseDto>()
                };
            }

            // Paginate the payments
            var paginatedPayments = payments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponseData = paginatedPayments.Select(payment => new PaymentResponseDto
            {
                Id = payment.Id,
                PayerFullName = $"{payment.User.FirstName} {payment.User.LastName}",
                Amount = payment.Amount,
                PaymentRef = payment.PaymentRef,
                PaymentFor = payment.ReasonForPayment,
                CreatedAt = payment.CreatedOn,
                Status = payment.Status
            }).ToList();

            return new PagedResponse<IEnumerable<PaymentResponseDto>>
            {
                IsSuccessful = true,
                Message = "Payments Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
            };
        }

        public async Task<Core.DTOs.Responses.ApiResponse<string>> InitiatePaymentAsync(CreatePaymentRequestDto requestDto)
        {
            var loginUser = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(loginUser!);
            if (HasPendingApplication(user, requestDto.EntityId!.Value)) return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "You already have a pending application, hence you cannot apply for this at the moment"
            };

            var response = await GetFeeAndEntityNameAsync(requestDto.ReasonForPayment, requestDto.EntityId.GetValueOrDefault(), user);
            var transactionReference = Guid.NewGuid().ToString();
            TransactionInitializeRequest request = new TransactionInitializeRequest()
            {
                AmountInKobo = response.fee * 100, //to get the kobo value of the amount
                Email = user!.Email,
                Reference = transactionReference,
                Currency = "NGN",
                CallbackUrl = $"{_configuration["AngularUrl"]}/payments/verify?applicationId={requestDto.EntityId.GetValueOrDefault()}&reason={requestDto.ReasonForPayment}&reference={transactionReference}"
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
                IsSuccessful = false,
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

        private async Task<(int fee, string entityName)> GetFeeAndEntityNameAsync(string paymentReason, Guid entityId, User user)
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
            else if (paymentReason == "Examination")
            {
                var exam = await _examinationRepository.GetExaminationAsync(e => e.Id == entityId)
                    ?? throw new Exception("Exam not found");
                fee = exam.Fee;
                entityName = exam.ExamTitle;
            }
            else if (paymentReason == "Dues")
            {
                var userLevel = user.Level ?? throw new Exception("User is not registered");
                fee = userLevel.Dues;
                entityName = userLevel.Name;
            }
            return (fee, entityName);
        }

        public async Task<Core.DTOs.Responses.ApiResponse<string>> VerifyPaymentAsync(string referenceNo)
        {
            TransactionVerifyResponse response = PayStack.Transactions.Verify(referenceNo);
            if (response.Data == null) return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Payment verification failed: No response data."
            };

            var payment = await _paymentRepository.GetPaymentAsync(p => p.PaymentRef == referenceNo);
            if (payment == null) return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Payment not found for the provided reference number."
            };

            if (response.Data.Status == "success")
            {
                payment.Status = Core.Enums.PaymentStatus.Successful;
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveAsync();

                return new Core.DTOs.Responses.ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = response.Message // Success message from Paystack
                };
            }
            else if (response.Data.Status == "failed")
            {
                payment.Status = Core.Enums.PaymentStatus.Failed;
                _paymentRepository.Update(payment);
                await _unitOfWork.SaveAsync();

                return new Core.DTOs.Responses.ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = response.Message // Failure message from Paystack
                };
            }

            // Handle other cases such as "pending" or unexpected status
            return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = $"Payment verification returned unexpected status: {response.Data.Status}."
            };
        }

        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> SearchPaymentAsync(SearchQueryRequestDto request)
        {
            var payments = await _paymentRepository.GetPaymentsAsync();
            var searchedPayments = payments.Where(payment =>
                                !string.IsNullOrEmpty(request.SearchQuery) &&
                                (payment.User.FirstName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || payment.User.LastName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || payment.ReasonForPayment.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || payment.PaymentRef.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || payment.Status.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase))
                                || (decimal.TryParse(request.SearchQuery, out decimal amount) && payment.Amount == amount)
                            ).ToList();


            if (!searchedPayments.Any()) return new PagedResponse<IEnumerable<PaymentResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageSize = request.PageSize > 0 ? request.PageSize : 5;
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var totalRecords = searchedPayments.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);


            var paginatedPayments = searchedPayments
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedPayments.Select(payment => new PaymentResponseDto
            {
                Id = payment.Id,
                PayerFullName = $"{payment.User.FirstName} {payment.User.LastName}",
                Amount = payment.Amount,
                PaymentRef = payment.PaymentRef,
                PaymentFor = payment.ReasonForPayment,
                CreatedAt = payment.CreatedOn,
                Status = payment.Status
            }).ToList();

            return new PagedResponse<IEnumerable<PaymentResponseDto>>
            {
                IsSuccessful = true,
                Message = "Payments Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }

        private bool HasPendingApplication(User user, Guid entityId)
        {
            return user.Applications
                .Any(application =>
                    application.ApplicationId == entityId &&
                    application.Status == Core.Enums.ApplicationStatus.Pending);
        }
    }
}

using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Enums;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
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
        private readonly IAppApplicationRepository _appApplicationRepository;
        private readonly IAppApplicationService _appApplicationService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly ILevelRepository _levelRepository;
        private readonly IEmailService _emailService;
        private readonly string token;

        private PayStackApi PayStack { get; set; }
        public PaymentService(IHttpContextAccessor httpContextAccessor, IPaymentRepository paymentRepository, IUserService userService,
            IConfiguration configuration, IUnitOfWork unitOfWork, IExaminationRepository examinationRepository, IEmailService emailService,
            IAppApplicationService appApplicationService, ITrainingRepository trainingRepository, UserManager<User> userManager,
            IAppApplicationRepository appApplicationRepository, ITokenService tokenService, IAuthService authService, ILevelRepository levelRepository
            )
        {
            _contextAccessor = httpContextAccessor;
            _paymentRepository = paymentRepository;
            _trainingRepository = trainingRepository;
            _examinationRepository = examinationRepository;
            _configuration = configuration;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _appApplicationService = appApplicationService;
            _appApplicationRepository = appApplicationRepository;
            _levelRepository = levelRepository;
            token = _configuration["Payment:PaystackSK"]!;
            PayStack = new PayStackApi(token);
            _tokenService = tokenService;
            _authService = authService;         
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
                    PaymentFor = payment.PaymentFor,
                    CreatedAt = payment.CreatedOn.ToString("D"),
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
                    PaymentFor = payment.PaymentFor,
                    CreatedAt = payment.CreatedOn.ToString("D"),
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
                PaymentFor = payment.PaymentFor,
                CreatedAt = payment.CreatedOn.ToString("D"),
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
            var user = await _authService.AuthenticateUserAsync(requestDto.Token);
            if (user == null)
            {
                return new Core.DTOs.Responses.ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "Authentication failed. Please log in or provide a valid token."
                };
            }

            var hasPendingApplication = HasPendingApplication(user, requestDto.ServiceId, requestDto.PaymentType);
            if (hasPendingApplication) return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "You already have a pending application, hence you cannot apply for this at the moment."
            };

            var response = await GetServiceAsync(requestDto.PaymentType, requestDto.ServiceId, user);
            var transactionReference = Guid.NewGuid().ToString();

            // Initialize PayStack transaction
            var payStackResponse = PayStack.Transactions.Initialize(new TransactionInitializeRequest
            {
                AmountInKobo = (int)(response.fee * 100),
                Email = user.Email!,
                Reference = transactionReference,
                Currency = "NGN",
                CallbackUrl = $"{_configuration["AngularUrl"]}/payments/verify?applicationId={requestDto.ServiceId}&reason={requestDto.PaymentType}&reference={transactionReference}"
            });

            if (!payStackResponse.Status) return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = payStackResponse.Message
            };

            // Map payment type
            var paymentFor = requestDto.PaymentType switch
            {
                PaymentType.Training => (response.service as Training)?.Title ?? "Training Fee",
                PaymentType.Examination => (response.service as Examination)?.ExamTitle ?? "Exam Fee",
                PaymentType.Application => (response.service as AppApplication)?.AppliedFor ?? "Application Fee",
                PaymentType.Dues => "Membership Dues",
                PaymentType.Registration => "Registration Fee",
                _ => throw new InvalidOperationException("Invalid payment type")
            };

            var payment = new Payment
            {
                UserId = user.Id,
                ServiceId = requestDto.ServiceId,
                User = user,
                Amount = response.fee,
                PaymentRef = transactionReference,
                PaymentType = requestDto.PaymentType,
                PaymentFor = paymentFor!,
                CreatedBy = user.Email!,
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

            //payment.PaymentType = request.ReasonForPayment ?? payment.PaymentType;
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
                    PaymentFor = payment.PaymentFor,
                    CreatedAt = payment.CreatedOn.ToString("D"),
                    Status = payment.Status
                }
            };
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


            payment.Status = response.Data.Status switch
            {
                "success" => PaymentStatus.Successful,
                "failed" => PaymentStatus.Failed,
                _ => payment.Status
            };
            _paymentRepository.Update(payment);

            if (response.Data.Status == "success")
            {
                var serviceName = await FlagServiceHasPaidAsync(payment.PaymentType, payment.ServiceId, payment.User);
                string token = _tokenService.GeneratePaymentToken(referenceNo);
                string url = $"{_configuration["AngularUrl"]}/payments-user?token={token}";

                if (serviceName == "Application")
                {
                    _appApplicationService.ApplicationPaymentConfirmation(payment, url);
                }
                else
                    PaymentConfirmationEmail(payment, url);
                
            }
            await _unitOfWork.SaveAsync();
            return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = response.Data.Status == "success",
                Message = response.Data.Status == "success" ? response.Message : response.Data.Status == "failed"
                ? response.Message
                : $"Payment verification returned unexpected status: {response.Data.Status}."
            };


        }

        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> SearchPaymentAsync(SearchQueryRequestDto request)
        {
            var payments = await _paymentRepository.GetPaymentsAsync();
            var searchedPayments = payments.Where(payment =>
                                !string.IsNullOrEmpty(request.SearchQuery) &&
                                (payment.User.FirstName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || payment.User.LastName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || payment.PaymentType.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
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

            var response = PaginatedResponse(request.PageSize, request.PageNumber, searchedPayments);
            return response;
        }

        private bool HasPendingApplication(User user, Guid serviceId, PaymentType paymentType)
        {
            ArgumentNullException.ThrowIfNull(user);
            if (user.Applications == null) return false;

            if (paymentType != PaymentType.Application)
                return false;

            if (serviceId == Guid.Empty)
                throw new ArgumentException("Service ID cannot be empty for payment types other than Dues.", nameof(serviceId));

            var userApplications = _appApplicationRepository.GetApplicationsAsync(user);
            return userApplications
                            .Any(app =>
                            app.Status == ApplicationStatus.Pending && app.HasPaid == true);
        }

        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> GetUserPaymentsAsync(int pageSize, int pageNumber)
        {
            var loginUser = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(loginUser!);
            var userPayments = await _paymentRepository.GetPaymentsAsync(user!);
            if (!userPayments.Any()) return new PagedResponse<IEnumerable<PaymentResponseDto>>
            {
                IsSuccessful = false,
                Message = "You have no payments",
                Data = null
            };

            var response = PaginatedResponse(pageSize, pageNumber, userPayments.ToList());
            return response;
        }


        private static PagedResponse<IEnumerable<PaymentResponseDto>> PaginatedResponse(int requestpageSize, int requestPageNumber, List<Payment> payments)
        {
            int pageSize = requestpageSize > 0 ? requestpageSize : 5;
            int pageNumber = requestPageNumber > 0 ? requestPageNumber : 1;

            var totalRecords = payments.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / requestpageSize);


            var paginatedPayments = payments
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedPayments.Select(payment => new PaymentResponseDto
            {
                Id = payment.Id,
                PayerFullName = $"{payment.User.FirstName} {payment.User.LastName}",
                Amount = payment.Amount,
                PaymentRef = payment.PaymentRef,
                PaymentFor = payment.PaymentFor,
                CreatedAt = payment.CreatedOn.ToString("D"),
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


        private async Task<(decimal fee, object? service)> GetServiceAsync(PaymentType paymentType, Guid serviceId, User user)
        {
            var level = await _levelRepository.GetLevelAsync(level => level.Id == user.LevelId);

            switch (paymentType)
            {
                case PaymentType.Training:
                    var training = await _trainingRepository.GetTrainingAsync(t => t.Id == serviceId)
                        ?? throw new InvalidOperationException("Training not found");
                    return (training.Fee, training);

                case PaymentType.Examination:
                    var examination = await _examinationRepository.GetExaminationAsync(e => e.Id == serviceId)
                        ?? throw new InvalidOperationException("Examination not found");
                    return (examination.Fee, examination);

                case PaymentType.Dues:
                    if (level == null ) throw new InvalidOperationException("User level not registered");
                    var userLevel = level;
                    return (userLevel.Dues, null);

                case PaymentType.Application:
                    var application = await _appApplicationRepository.GetApplicationAsync(a => a.Id == serviceId)
                          ?? throw new InvalidOperationException("Application not found");
                    return (application.ApplicationFee, application);

                case PaymentType.Registration:
                    return (100000, null);


                default:
                    throw new ArgumentException("Invalid payment type", nameof(paymentType));
            }
        }


        private async Task<string> FlagServiceHasPaidAsync(PaymentType paymentType, Guid serviceId, User user)
        {
            var level = await _levelRepository.GetLevelAsync(level => level.Id == user.LevelId);

            switch (paymentType)
            {
                case PaymentType.Training:
                    var training = await _trainingRepository.GetTrainingAsync(t => t.Id == serviceId)
                        ?? throw new InvalidOperationException("Training not found");
                    training.Haspaid = true;
                    _trainingRepository.Update(training);
                    return ("Training");

                case PaymentType.Examination:
                    var examination = await _examinationRepository.GetExaminationAsync(e => e.Id == serviceId)
                        ?? throw new InvalidOperationException("Examination not found");
                    examination.Haspaid = true;
                    _examinationRepository.Update(examination);
                    return ("Examination");

                case PaymentType.Dues:
                    var userLevel = level ?? throw new InvalidOperationException("User level not registered");
                    user.HasPaidDues = true;
                    await _userManager.UpdateAsync(user);
                    return ("Dues");

                case PaymentType.Application:
                    var application = await _appApplicationRepository.GetApplicationAsync(a => a.Id == serviceId);
                    application.HasPaid = true;
                    _appApplicationRepository.Update(application);
                    return ("Application");

                default:
                    throw new ArgumentException("Invalid payment type", nameof(paymentType));
            }
        }

        private void PaymentConfirmationEmail(Payment payment, string url)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Fullname", payment.User.FirstName + " " + payment.User.LastName },
                { "Amount", payment.Amount.ToString("C2", new System.Globalization.CultureInfo("en-NG")) },
                { "Service", $"Payment for {payment.PaymentFor}"},
                { "Date", payment.CreatedOn.ToString("D") },
                { "ReferenceNo", payment.PaymentRef },
                { "InstituteEmail", _configuration["InstituteEmail"]!},
                { "PaymentDetail", url },
                { "Sendername", "Ajala Abdbasit" },
                { "Level", "Admin" }
            };

            _emailService.SendEmail(
                 "PaymentConfirmationEmail.html",
                 replacements, payment.User.Email!,
                 payment.User.FirstName + " " + payment.User.LastName,
                 "Payment Confirmation"
                 );
        }
    }
}

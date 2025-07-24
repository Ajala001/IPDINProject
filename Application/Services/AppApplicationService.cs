using App.Application.HtmlFormat;
using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Enums;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using PaperKind = DinkToPdf.PaperKind;

namespace App.Application.Services
{
    public class AppApplicationService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager,
        IAppApplicationRepository applicationRepository, IUnitOfWork unitOfWork, ITrainingRepository trainingRepository,
        IExaminationRepository examinationRepository, IConverter converter, IApplicationSlip applicationSlip,
        IEmailService emailService, IConfiguration configuration, ITokenService tokenService) 
        : IAppApplicationService
    {
        public async Task<ApiResponse<AppApplicationResponseDto>> CreateAsync(CreateAppApplicationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(loginUser!);

            var serviceFee = (request.IsTraining
            ? (await trainingRepository.GetTrainingAsync(t => t.Id == request.ServiceId))?.ApplicationFee
            : (await examinationRepository.GetExaminationAsync(e => e.Id == request.ServiceId))?.ApplicationFee) 
                ?? throw new InvalidOperationException("Service not found for the provided ServiceId.");

            var appliedFor = await GetApplicationTitleAsync(request);
            var appApplication = new AppApplication
            {
                Id = Guid.NewGuid(),
                UserId = user!.Id,
                User = user,
                TrainingId = request.IsTraining ? request.ServiceId : Guid.Empty,
                ExaminationId = request.IsTraining ? Guid.Empty : request.ServiceId,
                AppliedFor = appliedFor,
                ApplicationFee = serviceFee,
                Status = ApplicationStatus.Pending,
                DateApplied = DateTime.Now,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow
            };

            await applicationRepository.CreateAsync(appApplication);
            await unitOfWork.SaveAsync();

            return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = true,
                Message = "Application Submitted Successfully",
                Data = new AppApplicationResponseDto
                {
                    Id = appApplication.Id,
                    ApplicantFullName = $"{user.FirstName} {user.LastName}",
                    AppliedFor = appliedFor,
                    Date = appApplication.DateApplied.ToString("D"),
                    Status = appApplication.Status,
                    HasPaid = appApplication.HasPaid ? "Yes" : "No"
                }
            };
        }

       
        public async Task<ApiResponse<AppApplicationResponseDto>> DeleteAsync(Guid id)
        {
            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new Core.DTOs.Responses.ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = false,
                Message = "Application Not Found",
                Data = null
            };

            applicationRepository.Delete(application);
            await unitOfWork.SaveAsync();

            return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = true,
                Message = "Application Deleted Successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<AppApplicationResponseDto>> GetAppApplicationAsync(Guid id)
        {
            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new Core.DTOs.Responses.ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = false,
                Message = "Application Not Found",
                Data = null
            };

            return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = true,
                Message = "Application Retrieved Successfully",
                Data = new AppApplicationResponseDto
                {
                    Id = application.Id,
                    ApplicantFullName = $"{application.User.FirstName} {application.User.LastName}",
                    AppliedFor = application.AppliedFor,
                    Date = application.DateApplied.ToString("D"),
                    Status = application.Status,
                    HasPaid = application.HasPaid ? "Yes" : "No"
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> GetAppApplicationsAsync(int pageSize, int pageNumber)
        {
            var applications = await applicationRepository.GetApplicationsAsync().ToListAsync();
            if (!applications.Any())
            {
                return new PagedResponse<IEnumerable<AppApplicationResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "Applications Not Found",
                    Data = null
                };
            }

            // If pageSize and pageNumber are not provided (null or 0), return all courses without pagination
            if (pageSize == 0 || pageNumber == 0)
            {
                var responseData = applications.Select(application => new AppApplicationResponseDto
                {
                    Id = application.Id,
                    ApplicantFullName = $"{application.User.FirstName} {application.User.LastName}",
                    AppliedFor = application.AppliedFor,
                    Date = application.DateApplied.ToString("D"),
                    Status = application.Status,
                    HasPaid = application.HasPaid ? "Yes" : "No"
                }).ToList();

                return new PagedResponse<IEnumerable<AppApplicationResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Applications Retrieved Successfully",
                    TotalRecords = applications.Count(),
                    Data = responseData
                };
            }
            var response = PaginatedResponse(pageSize, pageNumber, applications);
            return response;
        }


        public async Task<ApiResponse<AppApplicationResponseDto>> UpdateAsync(Guid id, UpdateAppApplicationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new Core.DTOs.Responses.ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = false,
                Message = "Application Not Found",
                Data = null
            };

            application.ModifiedBy = loginUser;
            application.ModifiedOn = DateTime.UtcNow;

            applicationRepository.Update(application);
            await unitOfWork.SaveAsync();

            return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = true,
                Message = "Application Updated Successfully",
                Data = new AppApplicationResponseDto
                {
                    Id = application.Id,
                    ApplicantFullName = $"{application.User.FirstName} {application.User.LastName}",
                    AppliedFor = application.AppliedFor,
                    Date = application.DateApplied.ToString("D"),
                    Status = application.Status,
                    HasPaid = application.HasPaid ? "Yes" : "No"
                }
            };
        }

        public async Task<ApiResponse<byte[]>> GenerateApplicationSlipAsync(Guid applicationId)
        {
            var application = await applicationRepository.GetApplicationAsync(a => a.Id == applicationId);
            var htmlContent = await applicationSlip.HtmlContent(application);

            var document = new HtmlToPdfDocument
            {
                GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4
            },
                Objects = {
                new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true }
                }
            }
            };

            return new ApiResponse<byte[]>
            {
                IsSuccessful = true,
                Message = "Successfully Generated",
                Data = converter.Convert(document)
            }; 
        }

        public async Task<ApiResponse<AppApplicationResponseDto>> AcceptApplicationAsync(Guid id)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var admin = await userManager.FindByEmailAsync(loginUser!); 
                        ArgumentException.ThrowIfNullOrEmpty(loginUser, nameof(loginUser));

            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = false,
                Message = "Application not found"
            };

            application.Status = ApplicationStatus.Approved;
            await unitOfWork.SaveAsync();

            var serviceId = application.ExaminationId != Guid.Empty ? application.ExaminationId
                                : (application.TrainingId != Guid.Empty ? application.TrainingId : Guid.Empty);

            var paymentType = application.ExaminationId != Guid.Empty ? PaymentType.Examination : PaymentType.Training;

            var token = tokenService.GeneratePaymentToken(application.User, serviceId, paymentType);
            string paymentUrl = $"{configuration["AngularUrl"]}/services/{serviceId}/detail?paymentType={paymentType}&token={token}";
            var replacements = new Dictionary<string, string>
            {
                { "Applicant", application.User.FirstName + "" + application.User.LastName },
                { "ApplicationPurpose", application.AppliedFor },
                { "PaymentUrl", paymentUrl },
                { "InstituteEmail", configuration["InstituteEmail"]! },
                { "Sendername", admin!.FirstName + " " + admin.LastName },
                { "Level", "Admin"}
            };
            var receiverEmail = application.User.Email;
            var receiverName = application.User.FirstName + " " + application.User.LastName;

            emailService.SendEmail(
                 "ApplicationAcceptanceEmail.html",
                 replacements, receiverEmail!,
                 receiverName,
                 "Your Application Status"
                 );

            return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = true,
                Message = "Application Accepted Successfully"
            };
        }

        public async Task<ApiResponse<string>> RejectApplicationAsync(Guid id, RejectionApplicationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var admin = await userManager.FindByEmailAsync(loginUser!);
                        ArgumentException.ThrowIfNullOrEmpty(loginUser, nameof(loginUser));

            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new Core.DTOs.Responses.ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Application not found"
            };

            application.Status = ApplicationStatus.Rejected;
            await unitOfWork.SaveAsync();
            
            string otherServices = $"{configuration["AngularUrl"]}/services";
            var replacements = new Dictionary<string, string>
            {
                { "Applicant", application.User.FirstName + "" + application.User.LastName },
                { "ApplicationPurpose", application.AppliedFor },
                { "RejectionReason", request.RejectionReason },
                { "InstituteEmail", configuration["InstituteEmail"]! },
                { "OtherServices", otherServices },
                { "Sendername", admin!.FirstName + " " + admin.LastName },
                { "Level", admin.Level!.Name}
            };
            var receiverEmail = application.User.Email;
            var receiverName = application.User.FirstName + " " + application.User.LastName;

            emailService.SendEmail(
                 "ApplicationRejectionEmail.html",
                 replacements, receiverEmail!,
                 receiverName,
                 "Your Application Status"
                 );

            return new ApiResponse<string>
            {
                IsSuccessful = true,
                Message = "Application Rejected",
                Data = request.RejectionReason
            };
        }

        public async Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> SearchApplicationAsync(SearchQueryRequestDto request)
        {
            var applications = applicationRepository.GetApplicationsAsync();
            var searchedApplications = await applications
                                    .Where(application =>
                                        !string.IsNullOrEmpty(request.SearchQuery) &&
                                        (
                                            application.User.FirstName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                            application.User.LastName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                            application.Status.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                            application.AppliedFor.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                        )
                                    )
                                    .ToListAsync();

            if (!searchedApplications.Any()) return new PagedResponse<IEnumerable<AppApplicationResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            var response = PaginatedResponse(request.PageSize, request.PageNumber, searchedApplications);
            return response;
        }

        public async Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> GetUserAppApplicationsAsync(int pageSize, int pageNumber)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(loginUser!);
            var userApplications = applicationRepository.GetApplicationsAsync(user);
            if (!userApplications.Any()) return new PagedResponse<IEnumerable<AppApplicationResponseDto>>
            {
                IsSuccessful = false,
                Message = "You have no applications",
                Data = null
            };

            var response = PaginatedResponse(pageSize, pageNumber, userApplications.ToList());
            return response;
        }


        private static PagedResponse<IEnumerable<AppApplicationResponseDto>> PaginatedResponse(int requestpageSize, int requestPageNumber, List<AppApplication> applications)
        {
            int pageSize = requestpageSize > 0 ? requestpageSize : 5;
            int pageNumber = requestPageNumber > 0 ? requestPageNumber : 1;

            var totalRecords = applications.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paginatedData = applications
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedData.Select(application => new AppApplicationResponseDto
            {
                Id = application.Id,
                ApplicantFullName = $"{application.User.FirstName} {application.User.LastName}",
                AppliedFor = application.AppliedFor,
                Date = application.DateApplied.ToString("D"),
                Status = application.Status,
                HasPaid = application.HasPaid ? "Yes" : "No"
            }).ToList();

            return new PagedResponse<IEnumerable<AppApplicationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Applications Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }

        
        private async Task<string> GetApplicationTitleAsync(CreateAppApplicationRequestDto request)
        {
            if (request.IsTraining)
            {
                var training = await trainingRepository.GetTrainingAsync(t => t.Id == request.ServiceId);
                return training?.Title ?? string.Empty;
            }

            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == request.ServiceId);
            return examination?.ExamTitle ?? string.Empty;
        }


        public void ApplicationPaymentConfirmation(Payment payment, string url)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Fullname", payment.User.FirstName + " " + payment.User.LastName },
                { "Amount", payment.Amount.ToString("C2", new System.Globalization.CultureInfo("en-NG")) },
                { "Service", $"Payment for {payment.PaymentFor}"},
                { "Date", payment.CreatedOn.ToString("D") },
                { "ReferenceNo", payment.PaymentRef },
                { "InstituteEmail", configuration["InstituteEmail"]!},
                { "PaymentDetail", url },
                { "Sendername", "Ajala Abdbasit" },
                { "Level", "Admin" }
            };

            emailService.SendEmail(
                 "PaymentConfirmationEmail.html",
                 replacements, payment.User.Email!,
                 payment.User.FirstName + " " + payment.User.LastName,
                 "Application Status"
                 );
        }
    }
}

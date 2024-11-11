using App.Application.HtmlFormat;
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
using System.Security.Claims;
using PaperKind = DinkToPdf.PaperKind;

namespace App.Application.Services
{
    public class AppApplicationService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager,
        IAppApplicationRepository applicationRepository, IUnitOfWork unitOfWork, ITrainingRepository trainingRepository,
        IExaminationRepository examinationRepository, IConverter converter, IApplicationSlip applicationSlip) 
        : IAppApplicationService
    {
        public async Task<ApiResponse<AppApplicationResponseDto>> CreateAsync(CreateAppApplicationRequestDto request)
       {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(loginUser!);

            var appApplication = new AppApplication
            {
                Id = Guid.NewGuid(),
                UserId = user!.Id,
                User = user,
                Status = ApplicationStatus.Pending,
                Date = DateTime.Now,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow
            };

            if (request.ApplicationType == "Training")
            {
                await HandleTrainingApplicationAsync(request.ApplicationId, appApplication);
            }
            else if (request.ApplicationType == "Examination")
            {
                await HandleExaminationApplicationAsync(request.ApplicationId, appApplication);
            }

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
                    ApplicationPurpose = appApplication.ApplicationPurpose,
                    Date = appApplication.Date,
                    Status = appApplication.Status
                }
            };
        }

        private async Task HandleTrainingApplicationAsync(Guid applicationId, AppApplication appApplication)
        {
            var training = await trainingRepository.GetTrainingAsync(tr => tr.Id == applicationId) 
                        ?? throw new InvalidOperationException("Training not found");
            appApplication.ApplicationId = applicationId;
            appApplication.ApplicationPurpose = training.Title;
        }

        private async Task HandleExaminationApplicationAsync(Guid applicationId, AppApplication appApplication)
        {
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == applicationId) 
                            ?? throw new InvalidOperationException("Examination not found");
            appApplication.ApplicationId = applicationId;
            appApplication.ApplicationPurpose = examination.ExamTitle;
        }

        public async Task<ApiResponse<AppApplicationResponseDto>> DeleteAsync(Guid id)
        {
            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new ApiResponse<AppApplicationResponseDto>
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
            if (application == null) return new ApiResponse<AppApplicationResponseDto>
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
                    ApplicationPurpose = application.ApplicationPurpose,
                    Date = application.Date,
                    Status = application.Status
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
                    ApplicationPurpose = application.ApplicationPurpose,
                    Date = application.Date,
                    Status = application.Status
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
            if (application == null) return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = false,
                Message = "Application Not Found",
                Data = null
            };

            application.ApplicationPurpose = request.ApplicationPurpose ?? application.ApplicationPurpose;
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
                    ApplicationPurpose = application.ApplicationPurpose,
                    Date = application.Date,
                    Status = application.Status
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
            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = false,
                Message = "Application not found"
            };

            application.Status = ApplicationStatus.Accepted;
            await unitOfWork.SaveAsync();

            return new ApiResponse<AppApplicationResponseDto>
            {
                IsSuccessful = true,
                Message = "Application Accepted Successfully"
            };
        }

        public async Task<ApiResponse<string>> RejectApplicationAsync(Guid id, RejectionApplicationRequestDto request)
        {
            var application = await applicationRepository.GetApplicationAsync(app => app.Id == id);
            if (application == null) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Application not found"
            };

            application.Status = ApplicationStatus.Rejected;
            await unitOfWork.SaveAsync();

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
                                            (application.ApplicationPurpose != null && application.ApplicationPurpose.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase))
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
            var totalPages = (int)Math.Ceiling((double)totalRecords / requestpageSize);

            var paginatedData = applications
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedData.Select(application => new AppApplicationResponseDto
            {
                Id = application.Id,
                ApplicantFullName = $"{application.User.FirstName} {application.User.LastName}",
                ApplicationPurpose = application.ApplicationPurpose,
                Date = application.Date,
                Status = application.Status
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
    }
}

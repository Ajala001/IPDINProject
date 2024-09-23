using App.Application.HtmlFormat;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
                ExaminationId = request.ExaminationId,
                Status = Core.Enums.ApplicationStatus.Pending,
                Date = DateTime.Now,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow
            };
            if(request.TrainingId != null)
            {
                appApplication.TrainingId = request.TrainingId;
                var training = await trainingRepository.GetTrainingAsync(tr => tr.Id == request.TrainingId);
                appApplication.ApplicationPurpose = training.Title;
            } else
                appApplication.ExaminationId = request.ExaminationId;
                var examination = await examinationRepository.GetExaminationAsync(e => e.Id == request.ExaminationId);
                appApplication.ApplicationPurpose = examination.ExamTitle;

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

        public async Task<ApiResponse<IEnumerable<AppApplicationResponseDto>>> GetAppApplicationsAsync()
        {
            var applications = await applicationRepository.GetApplicationsAsync();
            if (!applications.Any()) return new ApiResponse<IEnumerable<AppApplicationResponseDto>>
            {
                IsSuccessful = false,
                Message = "Applications Not Found",
                Data = null
            };

            var responseData = applications.Select(application =>
            {
                return new AppApplicationResponseDto
                {
                    Id = application.Id,
                    ApplicantFullName = $"{application.User.FirstName} {application.User.LastName}",
                    ApplicationPurpose = application.ApplicationPurpose,
                    Date = application.Date,
                    Status = application.Status
                };
            }).ToList();

            return new ApiResponse<IEnumerable<AppApplicationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Applications Retrieved Successfully",
                Data = responseData
            };
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
            application.Status = request.Status ?? application.Status;
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
    }
}

using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System.Security.Claims;
using System.Text.Json;

namespace App.Application.Services
{
    public class ResultService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor,
        IExaminationRepository examinationRepository, IResultRepository resultRepository, 
        IUnitOfWork unitOfWork, ILogger<ResultService> logger) : IResultService
    {
        
        public async Task<ApiResponse<ResultResponseDto>> DeleteAsync(string membershipNumber)
        {
            var result = await resultRepository.GetResultAsync(r => r.User.MembershipNumber == membershipNumber);
            if (result == null) return new ApiResponse<ResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            resultRepository.Delete(result);
            await unitOfWork.SaveAsync();

            return new ApiResponse<ResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result deleted successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<ResultResponseDto>> GetResultAsync(string membershipNumber)
        {
            var result = await resultRepository.GetResultAsync(r => r.User.MembershipNumber == membershipNumber);
            if (result == null) return new ApiResponse<ResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            return new ApiResponse<ResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result retrieved successfully",
                Data = new ResultResponseDto
                {
                    Id = result.Id,
                    FullName = $"{result.User.FirstName} {result.User.LastName}",
                    ExamTitle = result.Examination.ExamTitle,
                    TotalScore = result.TotalScore,
                    Breakdown = result.Breakdown
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<ResultResponseDto>>> GetResultsAsync()
        {
            var results = await resultRepository.GetResultsAsync();
            if (!results.Any()) return new ApiResponse<IEnumerable<ResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "Results not found",
                Data = null
            };

            var responseData = results.Select(result => new ResultResponseDto
            {
                Id = result.Id,
                FullName = $"{result.User.FirstName} {result.User.LastName}",
                ExamTitle = result.Examination.ExamTitle,
                TotalScore = result.TotalScore,
                Breakdown = result.Breakdown
            });

            return new ApiResponse<IEnumerable<ResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results retrieved successfully",
                Data = responseData
            };
        }

        public Task<ApiResponse<ResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<IEnumerable<ResultResponseDto>>> UploadResultAsync(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            logger.LogInformation("UploadResultAsync started by {LoginUser}", loginUser);

            if (file == null || file.Length == 0)
            {
                logger.LogWarning("File not provided or empty by {LoginUser}", loginUser);
                return new ApiResponse<IEnumerable<ResultResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "File not provided or empty",
                    Data = null
                };
            }

            var results = new List<Result>();
            var resultsDto = new List<ResultResponseDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume the first worksheet
                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text))
                        {
                            logger.LogInformation("Empty row encountered at row {RowNumber}, stopping processing.", row);
                            break;
                        }

                        var email = worksheet.Cells[row, 1].Text;
                        var examTitle = worksheet.Cells[row, 2].Text;
                        var totalScore = int.Parse(worksheet.Cells[row, 3].Text);
                        var breakdownJson = worksheet.Cells[row, 4].Text;

                        var user = await userManager.FindByEmailAsync(email);
                        if (user == null)
                        {
                            logger.LogWarning("User not found for email {Email} at row {RowNumber}", email, row);
                            continue;
                        }

                        var examination = await examinationRepository.GetExaminationAsync(e => e.ExamTitle == examTitle);
                        if (examination == null)
                        {
                            logger.LogWarning("Examination not found for title {ExamTitle} at row {RowNumber}", examTitle, row);
                            continue;
                        }

                        if (!IsValidJson(breakdownJson))
                        {
                            logger.LogWarning("Invalid JSON format in breakdown at row {RowNumber}", row);
                            continue;
                        }

                        var result = new Result
                        {
                            UserId = user.Id,
                            ExaminationId = examination.Id,
                            TotalScore = totalScore,
                            Breakdown = breakdownJson,
                            User = user,
                            Examination = examination,
                            CreatedOn = DateTime.Now,
                            CreatedBy = loginUser!
                        };
                        var fullName = $"{user.FirstName} {user.LastName}";
                        results.Add(result);
                        logger.LogInformation("Result added for user {fullName} and examination {ExamTitle}", fullName, examination.ExamTitle);

                        resultsDto.Add(new ResultResponseDto
                        {
                            Id = result.Id,
                            FullName = fullName,
                            ExamTitle = result.Examination.ExamTitle,
                            TotalScore = result.TotalScore,
                            Breakdown = result.Breakdown
                        });
                    }
                }
            }

            await resultRepository.UploadResultAsync(results);
            await unitOfWork.SaveAsync();

            logger.LogInformation("Results uploaded successfully by {LoginUser}", loginUser);

            return new ApiResponse<IEnumerable<ResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results Uploaded Successfully",
                Data = resultsDto
            };
        }

        private bool IsValidJson(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Invalid JSON encountered.");
                return false;
            }
        }
    }
}


using App.Core.DTOs.Requests.SearchRequestDtos;
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
    public class BatchResultService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor,
        IExaminationRepository examinationRepository, IUnitOfWork unitOfWork, ILogger<ResultService> logger,
        IBatchResultRepository batchResultRepository) : IBatchResultService
    {
        public async Task<ApiResponse<BulkResultResponseDto>> DeleteAsync(Guid batchId)
        {
            var batchResult = await batchResultRepository.GetBatchResultAsync(br => br.Id == batchId);
            if (batchResult == null) return new ApiResponse<BulkResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Batch Results not found",
                Data = null
            };

            batchResultRepository.Delete(batchResult);
            await unitOfWork.SaveAsync();
            return new ApiResponse<BulkResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Batch Result Deleted Successfully",
            };
        }

        public async Task<ApiResponse<BulkResultResponseDto>> GetBatchResultAsync(Guid batchId)
        {
            var batchResult = await batchResultRepository.GetBatchResultAsync(br => br.Id == batchId);
            if (batchResult == null) return new ApiResponse<BulkResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Batch Results not found",
                Data = null
            };

            return new ApiResponse<BulkResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Batch Result Received Successfully",
                Data = new BulkResultResponseDto
                {
                    Id = batchResult.Id,
                    ExamTitle = batchResult.Examination.ExamTitle,
                    UploadDate = batchResult.CreatedOn.ToLongDateString(),
                    TotalStudents = batchResult.Results.Count,
                    StudentResults = batchResult.Results.Select(r => new StudentResultResponseDto
                    {
                        Id = r.Id,
                        FullName = $"{r.User.FirstName} {r.User.LastName}",
                        TotalScore = r.TotalScore,
                        Breakdown = r.Breakdown,
                        ExamTitle = r.BatchResult.Examination.ExamTitle,
                    }).ToList()
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<BulkResultResponseDto>>> GetBatchResultsAsync(int pageSize, int pageNumber)
        {
            var batchResults = await batchResultRepository.GetBatchResultsAsync();
            if (!batchResults.Any()) return new PagedResponse<IEnumerable<BulkResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "Batch Results not found",
                Data = null
            };

            if (pageSize == 0 || pageNumber == 0)
            {
                var responseData = batchResults.Select(br => new BulkResultResponseDto
                {
                    Id = br.Id,
                    ExamTitle = br.Examination.ExamTitle,
                    UploadDate = br.CreatedOn.ToLongDateString(),
                    TotalStudents = br.Results.Count,
                    StudentResults = br.Results.Select(r => new StudentResultResponseDto
                    {
                        Id = r.Id,
                        FullName = $"{r.User.FirstName} {r.User.LastName}",
                        TotalScore = r.TotalScore,
                        Breakdown = r.Breakdown,
                        ExamTitle = r.BatchResult.Examination.ExamTitle,
                    }).ToList()
                }).ToList();

                return new PagedResponse<IEnumerable<BulkResultResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Courses Retrieved Successfully",
                    TotalRecords = batchResults.Count(),
                    Data = responseData
                };
            }


            var totalRecords = batchResults.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // If pageNumber exceeds total pages, return an empty response
            if (pageNumber > totalPages)
            {
                return new PagedResponse<IEnumerable<BulkResultResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "No more batch result available",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = new List<BulkResultResponseDto>()
                };
            }

            // Paginate the courses
            var paginatedBatchResults = batchResults
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponseData = paginatedBatchResults.Select(br => new BulkResultResponseDto
            {
                Id = br.Id,
                ExamTitle = br.Examination.ExamTitle,
                UploadDate = br.CreatedOn.ToLongDateString(),
                TotalStudents = br.Results.Count,
                StudentResults = br.Results.Select(r => new StudentResultResponseDto
                {
                    Id = r.Id,
                    FullName = $"{r.User.FirstName} {r.User.LastName}",
                    TotalScore = r.TotalScore,
                    Breakdown = r.Breakdown,
                    ExamTitle = r.BatchResult.Examination.ExamTitle,
                }).ToList()
            }).ToList();

            return new PagedResponse<IEnumerable<BulkResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Batch Results Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
            };
        }

        public async Task<ApiResponse<BulkResultResponseDto>> UploadBatchResultAsync(IFormFile file, Guid examId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            logger.LogInformation("UploadResultAsync started by {LoginUser}", loginUser);

            if (file == null || file.Length == 0)
            {
                logger.LogWarning("File not provided or empty by {LoginUser}", loginUser);
                return new ApiResponse<BulkResultResponseDto>
                {
                    IsSuccessful = false,
                    Message = "File not provided or empty",
                    Data = null
                };
            }

            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == examId);
            if (examination == null)
            {
                logger.LogWarning("Examination not found for ExamId {ExamId} by {LoginUser}", examId, loginUser);
                return new ApiResponse<BulkResultResponseDto>
                {
                    IsSuccessful = false,
                    Message = "Examination not found",
                    Data = null
                };
            }

            var bulkResultResponse = new BulkResultResponseDto
            {
                Id = Guid.NewGuid(),
                ExamTitle = examination.ExamTitle,
                UploadDate = DateTime.Now.ToLongDateString()
            };

            // Create the batch result
            var batchResult = new BatchResult
            {
                Id = Guid.NewGuid(),
                ExaminationId = examination.Id,
                Examination = examination,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.Now,
                Results = new List<Result>() // Initialize the results collection
            };

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        logger.LogError("No worksheets found in the uploaded file by {LoginUser}", loginUser);
                        return new ApiResponse<BulkResultResponseDto>
                        {
                            IsSuccessful = false,
                            Message = "No worksheets found in the uploaded file",
                            Data = null
                        };
                    }

                    if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                    {
                        logger.LogWarning("No data found in the worksheet by {LoginUser}", loginUser);
                        return new ApiResponse<BulkResultResponseDto>
                        {
                            IsSuccessful = false,
                            Message = "No data found in the worksheet",
                            Data = null
                        };
                    }

                    for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                    {
                        if (string.IsNullOrWhiteSpace(worksheet.Cells[row, 1].Text))
                        {
                            logger.LogInformation("Empty row encountered at row {RowNumber}, stopping processing.", row);
                            break;
                        }

                        var firstname = worksheet.Cells[row, 1].Text;
                        var lastname = worksheet.Cells[row, 2].Text;
                        var email = worksheet.Cells[row, 3].Text;
                        var membershipNum = worksheet.Cells[row, 4].Text;
                        var level = worksheet.Cells[row, 5].Text;
                        var breakdownJson = worksheet.Cells[row, 6].Text;

                        var user = await userManager.FindByEmailAsync(email);
                        if (user == null)
                        {
                            logger.LogWarning("User not found for email {Email} at row {RowNumber}", email, row);
                            continue;
                        }

                        if (!IsValidJson(breakdownJson))
                        {
                            logger.LogWarning("Invalid JSON format in breakdown at row {RowNumber}", row);
                            continue;
                        }

                        var courseScores = JsonSerializer.Deserialize<Dictionary<string, object>>(breakdownJson);
                        double totalScore = 0;

                        foreach (var value in courseScores.Values)
                        {
                            if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Number)
                            {
                                totalScore += jsonElement.GetDouble();
                            }
                            else
                            {
                                Console.WriteLine($"Non-numeric value encountered: {value}");
                            }
                        }


                        // Create result
                        var result = new Result
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            TotalScore = totalScore, 
                            Breakdown = breakdownJson,
                            User = user,
                            BatchId = batchResult.Id,
                            BatchResult = batchResult,
                            CreatedOn = DateTime.Now,
                            CreatedBy = loginUser!
                        };

                        // Add result to the batch
                        batchResult.Results.Add(result);
                        batchResult.NumberOfUploadedResults = batchResult.Results.Count;
                        bulkResultResponse.UploadDate = batchResult.CreatedOn.ToLongDateString();
                        bulkResultResponse.StudentResults.Add(new StudentResultResponseDto
                        {
                            Id = result.Id,
                            FullName = $"{user.FirstName} {user.LastName}",
                            TotalScore = result.TotalScore,
                            Breakdown = result.Breakdown,
                            ExamTitle = examination.ExamTitle,
                        });

                        logger.LogInformation("Result added for user {FullName} and examination {ExamTitle}", $"{user.FirstName} {user.LastName}", examination.ExamTitle);
                    }
                }
            }

            // Save the batch result with all results
            await batchResultRepository.UploadBatchResultAsync(batchResult);
            bulkResultResponse.TotalStudents = batchResult.Results.Count;
            var response = await unitOfWork.SaveAsync();
            if(response > 0) return new ApiResponse<BulkResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Results Uploaded Successfully",
                Data = bulkResultResponse
            };

            return new ApiResponse<BulkResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Something went wrong",
                Data = bulkResultResponse
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


        public async Task<PagedResponse<IEnumerable<BulkResultResponseDto>>> SearchBatchResultAsync(SearchQueryRequestDto request)
        {
            var batchResults = await batchResultRepository.GetBatchResultsAsync();
            var searchBatchResults = batchResults
                                .Where(batchResult => string.IsNullOrEmpty(request.SearchQuery) ||
                                 (batchResult.ExaminationId.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                 || batchResult.CreatedOn.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)))
                                .ToList();

            if (!searchBatchResults.Any()) return new PagedResponse<IEnumerable<BulkResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageSize = request.PageSize > 0 ? request.PageSize : 5;
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var totalRecords = searchBatchResults.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);


            var paginatedBatchResults = searchBatchResults
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedBatchResults.Select(br => new BulkResultResponseDto
            {

                Id = br.Id,
                ExamTitle = br.Examination.ExamTitle,
                UploadDate = br.CreatedOn.ToLongDateString(),
                TotalStudents = br.Results.Count,
                StudentResults = br.Results.Select(r => new StudentResultResponseDto
                {
                    Id = r.Id,
                    FullName = $"{r.User.FirstName} {r.User.LastName}",
                    TotalScore = r.TotalScore,
                    Breakdown = r.Breakdown,
                    ExamTitle = r.BatchResult.Examination.ExamTitle,
                }).ToList()
            }).ToList();

            return new PagedResponse<IEnumerable<BulkResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Batch Results Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }

    }
}

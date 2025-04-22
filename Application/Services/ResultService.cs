using App.Application.HtmlFormat;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Application.Services
{
    public class ResultService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor,
        IResultRepository resultRepository, IBatchResultRepository batchResultRepository, IUnitOfWork unitOfWork,
         IResultFormat resultFormat, IConverter converter) : IResultService
    {
        
        public async Task<ApiResponse<StudentResultResponseDto>> DeleteAsync(Guid resultId)
        {
            var result = await resultRepository.GetResultAsync(r => r.Id == resultId);
            if (result == null) return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            resultRepository.Delete(result);
            await unitOfWork.SaveAsync();

            return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result deleted successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<StudentResultResponseDto>> GetResultAsync(Guid resultId)
        {
            var result = await resultRepository.GetResultAsync(r => r.Id == resultId);
            if (result == null) return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result retrieved successfully",
                Data = new StudentResultResponseDto
                {
                    Id = result.Id,
                    BatchId = result.BatchId,
                    ExamId = result.BatchResult.ExaminationId,
                    Email = result.User.Email!,
                    FullName = $"{result.User.FirstName} {result.User.LastName}",
                    PhoneNumber = result.User.PhoneNumber!,
                    MembershipNumber = result.User.MembershipNumber!,
                    ProfilePic = result.User.ProfilePic!,
                    TotalScore = result.TotalScore,
                    Breakdown = result.Breakdown
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> GetMemberResultsAsync(string membershipNumber, int pageSize, int pageNumber)
        {
            var user = await userManager.Users
                        .Where(u => u.MembershipNumber == membershipNumber)
                        .FirstOrDefaultAsync();

            if (user == null) return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            var results = await resultRepository.GetResultsAsync(user);
            if (results == null || !results.Any()) return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "No results found for the user",
                Data = null
            };

            pageSize = pageSize > 0 ? pageSize : 5;
            pageNumber = pageNumber > 0 ? pageNumber : 1;

            var totalRecords = results.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);


            var paginatedResults = results
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            // Map results to DTO
            var responseData = paginatedResults.Select(result => new StudentResultResponseDto
            {
                Id = result.Id,
                BatchId = result.BatchId,
                ExamId = result.BatchResult.ExaminationId,
                FullName = $"{result.User.FirstName} {result.User.LastName}",
                Email = result.User.Email!,
                PhoneNumber = result.User.PhoneNumber!,
                MembershipNumber = result.User.MembershipNumber!,
                ProfilePic = result.User.ProfilePic!,
                TotalScore = result.TotalScore,
                Breakdown = result.Breakdown
            }).ToList();

            // Return the successful response
            return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }


        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> GetBatchResultsAsync(Guid batchResultId, int pageSize, int pageNumber)
        {
            var batchResult = await batchResultRepository.GetBatchResultAsync(br => br.Id == batchResultId);
            if (batchResult == null) return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "Batch Result not found",
                Data = null
            };

            var results = await resultRepository.GetResultsAsync(batchResult);
            if (results == null || !results.Any()) return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "No results found for the batch",
                Data = null
            };

            pageSize = pageSize > 0 ? pageSize : 5;
            pageNumber = pageNumber > 0 ? pageNumber : 1;

            var totalRecords = results.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);


            var paginatedResults = results
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedResults.Select(result => new StudentResultResponseDto
            {
                Id = result.Id,
                BatchId = result.BatchId,
                ExamId = result.BatchResult.ExaminationId,
                FullName = $"{result.User.FirstName} {result.User.LastName}",
                Email = result.User.Email!,
                PhoneNumber = result.User.PhoneNumber!,
                MembershipNumber = result.User.MembershipNumber!,
                ProfilePic = result.User.ProfilePic!,
                TotalScore = result.TotalScore,
                Breakdown = result.Breakdown
            }).ToList();

            return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }


        public async Task<ApiResponse<StudentResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var result = await resultRepository.GetResultAsync(r => r.User.MembershipNumber == membershipNumber
                            && r.BatchResult.ExaminationId == request.ExaminationId);

            if(result == null) return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            result.TotalScore = request.TotalScore ?? result.TotalScore;
            result.Breakdown = request.Breakdown ?? result.Breakdown;
            result.ModifiedBy = loginUser;
            result.ModifiedOn = DateTime.Now;

            resultRepository.Update(result);
            await unitOfWork.SaveAsync();

            return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result updated successfully",
                Data = new StudentResultResponseDto
                {
                    Id = result.Id,
                    BatchId = result.BatchId,
                    ExamId = result.BatchResult.ExaminationId,
                    FullName = $"{result.User.FirstName} {result.User.LastName}",
                    Email = result.User.Email!,
                    PhoneNumber = result.User.PhoneNumber!,
                    MembershipNumber = result.User.MembershipNumber!,
                    ProfilePic = result.User.ProfilePic!,
                    TotalScore = result.TotalScore,
                    Breakdown = result.Breakdown
                }
            };
        }


        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> SearchResultAsync(SearchQueryRequestDto request)
        {
            var results = await resultRepository.GetResultsAsync();
            var searchResults = results
                                .Where(result => string.IsNullOrEmpty(request.SearchQuery) ||
                                    result.BatchResult.ExaminationId.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    result.TotalScore.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    result.User.FirstName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    result.User.LastName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase))
                                .ToList();

            if (!searchResults.Any()) return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageSize = request.PageSize > 0 ? request.PageSize : 5;
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var totalRecords = searchResults.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);


            var paginatedResults = searchResults
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedResults.Select(result => new StudentResultResponseDto
            {
                Id = result.Id,
                BatchId = result.BatchId,
                ExamId = result.BatchResult.ExaminationId,
                FullName = $"{result.User.FirstName} {result.User.LastName}",
                Email = result.User.Email!,
                PhoneNumber = result.User.PhoneNumber!,
                MembershipNumber = result.User.MembershipNumber!,
                ProfilePic = result.User.ProfilePic!,
                TotalScore = result.TotalScore,
                Breakdown = result.Breakdown
            }).ToList();

            return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }

        public async Task<ApiResponse<byte[]>> GenerateResultAsync(Guid resultId)
        {
            var result = await resultRepository.GetResultAsync(r => r.Id == resultId);
            var htmlContent = await resultFormat.HtmlContent(result);

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

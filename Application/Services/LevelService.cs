using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace App.Application.Services
{
    public class LevelService(ILevelRepository levelRepository,
        IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : ILevelService
    {
        public async Task<ApiResponse<LevelRequestResponseDto>> CreateAsync(CreateLevelRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var existingLevel = await levelRepository.GetLevelAsync(r => r.Name == request.Name);
            if (existingLevel != null) return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = false,
                Message = $"{request.Name} level already exist"
            };

            var newLevel = new Level
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Dues = request.Dues,
                CreatedBy = loginUser,
                CreatedOn = DateTime.Now
            };
            await levelRepository.CreateAsync(newLevel);
            await unitOfWork.SaveAsync();

            return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = true,
                Message = $"{request.Name} created successfully"
            };
        }

        public async Task<ApiResponse<LevelRequestResponseDto>> DeleteAsync(Guid id)
        {
            var level = await levelRepository.GetLevelAsync(r => r.Id == id);
            if (level == null) return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = false,
                Message = "Level Not found"
            };
            
            levelRepository.Delete(level);
            await unitOfWork.SaveAsync();

            return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = true,
                Message = "Deleted successfully"
            };
        }

        public async Task<ApiResponse<LevelRequestResponseDto>> GetLevelAsync(Guid id)
        {
            var level = await levelRepository.GetLevelAsync(r => r.Id == id);
            if (level == null) return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = false,
                Message = "level Not found"
            };

            return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = true,
                Message = "Level Retrieved successfully",
                Data = new LevelRequestResponseDto
                {
                    Id = level.Id,
                    Name = level.Name,
                    Dues = level.Dues
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<LevelRequestResponseDto>>> GetLevelsAsync()
        {
            var levels = await levelRepository.GetLevelsAsync();
            if (!levels.Any()) return new ApiResponse<IEnumerable<LevelRequestResponseDto>>
            {
                IsSuccessful = false,
                Message = "Levels Not found"
            };

            var responseData = levels.Select(t => new LevelRequestResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Dues = t.Dues
            }).ToList();

            return new ApiResponse<IEnumerable<LevelRequestResponseDto>>
            {

                IsSuccessful = true,
                Message = "Levels Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<LevelRequestResponseDto>> UpdateAsync(Guid id, UpdateLevelRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var level = await levelRepository.GetLevelAsync(r => r.Id == id);
            if (level == null) return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = false,
                Message = "Level Not found"
            };


            level.Name = request.Name ?? level.Name;
            level.Dues = request.Dues ?? level.Dues;
            level.ModifiedBy = loginUser!;
            level.ModifiedOn = DateTime.Now;

            levelRepository.Update(level);
            await unitOfWork.SaveAsync();

            return new ApiResponse<LevelRequestResponseDto>
            {
                IsSuccessful = true,
                Message = "Level updated successfully",
                Data = new LevelRequestResponseDto
                {
                    Id = level.Id,
                    Name = level.Name,
                    Dues = level.Dues
                }
            };
        }
    }
}

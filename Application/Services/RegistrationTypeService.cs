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
    public class RegistrationTypeService(IRegistrationTypeRepository registrationTypeRepository,
        IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IRegistrationTypeService
    {
        public async Task<ApiResponse<RegistrationTypeResponseDto>> CreateAsync(CreateRegistrationTypeRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var existingType = await registrationTypeRepository.GetRegistrationTypeAsync(r => r.Name == request.Name);
            if (existingType != null) return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = false,
                Message = $"{request.Name} registration type already exist"
            };

            var newType = new RegistrationType
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Dues = request.Dues,
                CreatedBy = loginUser,
                CreatedOn = DateTime.Now
            };
            await registrationTypeRepository.CreateAsync(newType);
            await unitOfWork.SaveAsync();

            return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = true,
                Message = $"{request.Name} created successfully"
            };
        }

        public async Task<ApiResponse<RegistrationTypeResponseDto>> DeleteAsync(Guid id)
        {
            var type = await registrationTypeRepository.GetRegistrationTypeAsync(r => r.Id == id);
            if (type == null) return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = false,
                Message = "Type Not found"
            };
            
            registrationTypeRepository.Delete(type);
            await unitOfWork.SaveAsync();

            return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = true,
                Message = "Deleted successfully"
            };
        }

        public async Task<ApiResponse<RegistrationTypeResponseDto>> GetGetRegistrationTypeAsync(Guid id)
        {
            var type = await registrationTypeRepository.GetRegistrationTypeAsync(r => r.Id == id);
            if (type == null) return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = false,
                Message = "Type Not found"
            };

            return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = true,
                Message = "Registration type Retrieved successfully",
                Data = new RegistrationTypeResponseDto
                {
                    Id = type.Id,
                    Name = type.Name,
                    Dues = type.Dues
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<RegistrationTypeResponseDto>>> GetRegistrationTypesAsync()
        {
            var types = await registrationTypeRepository.GetRegistrationTypesAsync();
            if (!types.Any()) return new ApiResponse<IEnumerable<RegistrationTypeResponseDto>>
            {
                IsSuccessful = false,
                Message = "Types Not found"
            };

            var responseData = types.Select(t => new RegistrationTypeResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Dues = t.Dues
            }).ToList();

            return new ApiResponse<IEnumerable<RegistrationTypeResponseDto>>
            {

                IsSuccessful = true,
                Message = "Type Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<RegistrationTypeResponseDto>> UpdateAsync(Guid id, UpdateRegistrationTypeRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var type = await registrationTypeRepository.GetRegistrationTypeAsync(r => r.Id == id);
            if (type == null) return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = false,
                Message = "Type Not found"
            };


            type.Name = request.Name ?? type.Name;
            type.Dues = request.Dues ?? type.Dues;
            type.ModifiedBy = loginUser!;
            type.ModifiedOn = DateTime.Now;

            registrationTypeRepository.Update(type);
            await unitOfWork.SaveAsync();

            return new ApiResponse<RegistrationTypeResponseDto>
            {
                IsSuccessful = true,
                Message = "Registration type updated successfully",
                Data = new RegistrationTypeResponseDto
                {
                    Id = type.Id,
                    Name = type.Name,
                    Dues = type.Dues
                }
            };
        }
    }
}

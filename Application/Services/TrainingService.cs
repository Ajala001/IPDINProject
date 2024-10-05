using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Application.Services
{
    public class TrainingService(IHttpContextAccessor httpContextAccessor,
        ITrainingRepository trainingRepository, IUnitOfWork unitOfWork) : ITrainingService
    {
        public async Task<ApiResponse<TrainingResponseDto>> CreateAsync(CreateTrainingRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var training = await trainingRepository.GetTrainingAsync(t =>
                            t.Title == request.Title &&
                            t.Status != Core.Enums.TrainingStatus.Completed);
            if (training != null) return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = false,
                Message = "Training already exist and still going on",
                Data = null
            };

            var newTraining = new Training
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Fee = request.Fee,
                StartingDateAndTime = request.StartingDateAndTime,
                EndingDateAndTime = request.EndingDateAndTime,
                RegistrationDeadline = request.RegistrationDeadline,
                Duration = request.Duration,
                Capacity = request.Capacity,
                Category = request.Category,
                Status = request.Status,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow,
            };

            await trainingRepository.CreateAsync(newTraining);
            await unitOfWork.SaveAsync();

            return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = true,
                Message = "Training Scheduled Successfully",
                Data = new TrainingResponseDto
                {
                    Id = newTraining.Id,
                    Title = newTraining.Title,
                    Description = newTraining.Description,
                    Fee = newTraining.Fee,
                    StartingDateAndTime = newTraining.StartingDateAndTime,
                    EndingDateAndTime = newTraining.EndingDateAndTime,
                    RegistrationDeadline = request.RegistrationDeadline,
                    Duration = newTraining.Duration,
                    Capacity = newTraining.Capacity,
                    Category = newTraining.Category,
                    Status = newTraining.Status
                }
            };
        }

        public async Task<ApiResponse<TrainingResponseDto>> DeleteAsync(Guid id)
        {
            var training = await trainingRepository.GetTrainingAsync(t => t.Id == id);
            if (training == null) return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = false,
                Message = "Training not found",
                Data = null
            };

            trainingRepository.Delete(training);
            await unitOfWork.SaveAsync();

            return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = true,
                Message = "Training Deleted Successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<IEnumerable<TrainingResponseDto>>> GetTainingsAsync()
        {
            var trainings = await trainingRepository.GetTrainingsAsync();
            if (!trainings.Any()) return new ApiResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = false,
                Message = "Trainings Not Found",
                Data = null
            };

            var responseData = trainings.Select(t => new TrainingResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Fee = t.Fee,
                StartingDateAndTime = t.StartingDateAndTime,
                EndingDateAndTime = t.EndingDateAndTime,
                RegistrationDeadline = t.RegistrationDeadline,
                Duration = t.Duration,
                Capacity = t.Capacity,
                Category = t.Category,
                Status = t.Status
            }).ToList();

            return new ApiResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = true,
                Message = "Trainings Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<TrainingResponseDto>> GetTrainingAsync(Guid id)
        {
            var training = await trainingRepository.GetTrainingAsync(t => t.Id == id);
            if (training == null) return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = false,
                Message = "Training not found",
                Data = null
            };

            return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = true,
                Message = "Training Retrieved Successfully",
                Data = new TrainingResponseDto
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    Fee = training.Fee,
                    StartingDateAndTime = training.StartingDateAndTime,
                    EndingDateAndTime = training.EndingDateAndTime,
                    RegistrationDeadline = training.RegistrationDeadline,
                    Duration = training.Duration,
                    Capacity = training.Capacity,
                    Category = training.Category,
                    Status = training.Status
                }
            };

        }

        public async Task<ApiResponse<IEnumerable<TrainingResponseDto>>> SearchTrainingAsync(TrainingSearchRequestDto request)
        {
            var trainings = trainingRepository.Query();
            var trainingQuery = trainings.Where(training =>
            (!string.IsNullOrEmpty(request.TrainingTitle) && training.Title.Contains(request.TrainingTitle, StringComparison.OrdinalIgnoreCase))
            || (training.Category == request.Category) || (training.Status == request.Status)
             );

            var searchedTrainings = await trainingQuery.ToListAsync();
            if (!searchedTrainings.Any()) return new ApiResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageNumber = request.PageNumber;
            int pageSize = request.PageSize;
            var paginatedTrainings = searchedTrainings
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedTrainings.Select(t => new TrainingResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Fee = t.Fee,
                StartingDateAndTime = t.StartingDateAndTime,
                EndingDateAndTime = t.EndingDateAndTime,
                RegistrationDeadline = t.RegistrationDeadline,
                Duration = t.Duration,
                Capacity = t.Capacity,
                Category = t.Category,
                Status = t.Status
            }).ToList();

            return new ApiResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = true,
                Message = "Trainings Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<TrainingResponseDto>> UpdateAsync(Guid id, UpdateTrainingRequestDto request)
        {
            var training = await trainingRepository.GetTrainingAsync(t => t.Id == id);
            if (training == null) return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = false,
                Message = "Training not found",
                Data = null
            };

            training.Title = request.Title ?? training.Title;
            training.Description = request.Description ?? training.Description; 
            training.Fee = request.Fee ?? training.Fee;
            training.StartingDateAndTime = request.StartingDateAndTime ?? training.StartingDateAndTime;
            training.EndingDateAndTime = request.EndingDateAndTime ?? training.EndingDateAndTime;
            training.RegistrationDeadline = request.RegistrationDeadline ?? training.RegistrationDeadline;
            training.Duration = request.Duration ?? training.Duration;
            training.Capacity = request.Capacity ?? training.Capacity;
            training.Category = request.Category ?? training.Category;
            training.Status = request.Status ?? training.Status;

            trainingRepository.Update(training);
            await unitOfWork.SaveAsync();

            return new ApiResponse<TrainingResponseDto>
            {
                IsSuccessful = true,
                Message = "Training Updated Successfully",
                Data = new TrainingResponseDto
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    Fee = training.Fee,
                    StartingDateAndTime = training.StartingDateAndTime,
                    EndingDateAndTime = training.EndingDateAndTime,
                    RegistrationDeadline = training.RegistrationDeadline,
                    Duration = training.Duration,
                    Capacity = training.Capacity,
                    Category = training.Category,
                    Status = training.Status
                }
            };
        }
    }
}


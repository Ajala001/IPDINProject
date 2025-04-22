using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Application.Services
{
    public class TrainingService(IHttpContextAccessor httpContextAccessor, ITrainingRepository trainingRepository, 
        IUnitOfWork unitOfWork, UserManager<User> userManager) : ITrainingService
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
                ApplicationFee = request.Fee * 0.1m,
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
                    Fee = newTraining.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = newTraining.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
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

        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> GetTainingsAsync(int pageSize, int pageNumber)
        {
            var trainings = await trainingRepository.GetTrainingsAsync();
            if (trainings == null || !trainings.Any())
            {
                return new PagedResponse<IEnumerable<TrainingResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "Trainings Not Found",
                    Data = null
                };
            }

            // If pageSize and pageNumber are not provided (null or 0), return all courses without pagination
            if (pageSize == 0 || pageNumber == 0)
            {
                var responseData = trainings.Select(training => new TrainingResponseDto
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    Fee = training.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = training.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    StartingDateAndTime = training.StartingDateAndTime,
                    EndingDateAndTime = training.EndingDateAndTime,
                    RegistrationDeadline = training.RegistrationDeadline,
                    Duration = training.Duration,
                    Capacity = training.Capacity,
                    Category = training.Category,
                    Status = training.Status
                }).ToList();

                return new PagedResponse<IEnumerable<TrainingResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Trainings Retrieved Successfully",
                    TotalRecords = trainings.Count(),
                    Data = responseData
                };
            }


            var totalRecords = trainings.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // If pageNumber exceeds total pages, return an empty response
            if (pageNumber > totalPages)
            {
                return new PagedResponse<IEnumerable<TrainingResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "No more training available",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = new List<TrainingResponseDto>()
                };
            }

            // Paginate the trainings
            var paginatedTrainings = trainings
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponseData = paginatedTrainings.Select(training => new TrainingResponseDto
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                Fee = training.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                ApplicationFee = training.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                StartingDateAndTime = training.StartingDateAndTime,
                EndingDateAndTime = training.EndingDateAndTime,
                RegistrationDeadline = training.RegistrationDeadline,
                Duration = training.Duration,
                Capacity = training.Capacity,
                Category = training.Category,
                Status = training.Status
            }).ToList();

            return new PagedResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = true,
                Message = "Trainings Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
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
                Message = "Training retrievd Successfully",
                Data = new TrainingResponseDto
                {
                    Id = training.Id,
                    Title = training.Title,
                    Description = training.Description,
                    Fee = training.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = training.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
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

        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> SearchTrainingAsync(SearchQueryRequestDto request)
        {
            var trainings = await trainingRepository.GetTrainingsAsync();
            var searchedTrainings = trainings.Where(training =>
                                !string.IsNullOrEmpty(request.SearchQuery) &&
                                (training.Title.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || int.TryParse(request.SearchQuery, out int fee) && training.Fee == fee)
                                || training.Status.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || training.Category.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                            ).ToList();


            if (!searchedTrainings.Any()) return new PagedResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            var response = PaginatedResponse(request.PageSize, request.PageNumber, searchedTrainings);
            return response;
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
                    Fee = training.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = training.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
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

        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> GetUserTainingsAsync(int pageSize, int pageNumber)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(loginUser!);
            var usertrainings = await trainingRepository.GetTrainingsAsync(user!);
            if(!usertrainings.Any()) return new PagedResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            var response = PaginatedResponse(pageSize, pageNumber, usertrainings.ToList());
            return response;

        }

        private static PagedResponse<IEnumerable<TrainingResponseDto>> PaginatedResponse(int requestpageSize, int requestPageNumber, List<Training> trainings)
        {

            int pageSize = requestpageSize > 0 ? requestpageSize : 5;
            int pageNumber = requestPageNumber > 0 ? requestPageNumber : 1;

            var totalRecords = trainings.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);


            var paginatedTrainings = trainings
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedTrainings.Select(training => new TrainingResponseDto
            {
                Id = training.Id,
                Title = training.Title,
                Description = training.Description,
                Fee = training.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                ApplicationFee = training.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                StartingDateAndTime = training.StartingDateAndTime,
                EndingDateAndTime = training.EndingDateAndTime,
                RegistrationDeadline = training.RegistrationDeadline,
                Duration = training.Duration,
                Capacity = training.Capacity,
                Category = training.Category,
                Status = training.Status
            }).ToList();

            return new PagedResponse<IEnumerable<TrainingResponseDto>>
            {
                IsSuccessful = true,
                Message = "Trainings Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }
    }
}


using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.RegistrationType
{
    public record GetAllLevelsQuery() : IRequest<ApiResponse<IEnumerable<LevelRequestResponseDto>>>;

    public class GetAllRegistrationTypeQueryHandler(ILevelService levelService)
        : IRequestHandler<GetAllLevelsQuery, ApiResponse<IEnumerable<LevelRequestResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<LevelRequestResponseDto>>> Handle(GetAllLevelsQuery request, CancellationToken cancellationToken)
        {
            return await levelService.GetLevelsAsync();
        }
    }
}

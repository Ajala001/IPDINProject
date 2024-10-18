using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.RegistrationType
{
    public record GetLevelByIdQuery(Guid LevelId) : IRequest<ApiResponse<LevelRequestResponseDto>>;

    public class GetRegistrationTypeByIdQueryHandler(ILevelService levelService)
        : IRequestHandler<GetLevelByIdQuery, ApiResponse<LevelRequestResponseDto>>
{
        public async Task<ApiResponse<LevelRequestResponseDto>> Handle(GetLevelByIdQuery request, CancellationToken cancellationToken)
        {
            return await levelService.GetLevelAsync(request.LevelId);
        }
    }
}

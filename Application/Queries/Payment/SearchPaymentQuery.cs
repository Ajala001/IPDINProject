using App.Application.IExternalServices;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Queries.Payment
{
    public record SearchPaymentQuery(SearchQueryRequestDto SearchRequestDto)
       : IRequest<PagedResponse<IEnumerable<PaymentResponseDto>>>;

    public class SearchPaymentQueryHandler(IPaymentService paymentService)
        : IRequestHandler<SearchPaymentQuery, PagedResponse<IEnumerable<PaymentResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> Handle(SearchPaymentQuery request, CancellationToken cancellationToken)
        {
            return await paymentService.SearchPaymentAsync(request.SearchRequestDto);
        }
    }
}

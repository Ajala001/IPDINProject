using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Queries.Payment
{
    public record GetUserPaymentsQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<PaymentResponseDto>>>;

    public class GetUserPaymentsQueryHandler(IPaymentService paymentService)
        : IRequestHandler<GetUserPaymentsQuery, PagedResponse<IEnumerable<PaymentResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> Handle(GetUserPaymentsQuery request, CancellationToken cancellationToken)
        {
            return await paymentService.GetUserPaymentsAsync(request.PageSize, request.PageNumber);
        }
    }
}

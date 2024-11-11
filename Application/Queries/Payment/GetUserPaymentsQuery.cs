using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Queries.Payment
{
    public record GetUserPaymentsQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<PaymentResponseDto>>>;

    public class GetUserPaymentsQueryHandler(IPaymentService paymentService)
        : IRequestHandler<GetAllPaymentQuery, PagedResponse<IEnumerable<PaymentResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> Handle(GetAllPaymentQuery request, CancellationToken cancellationToken)
        {
            return await paymentService.GetUserPaymentsAsync(request.PageSize, request.PageNumber);
        }
    }
}

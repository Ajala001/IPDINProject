using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Queries.Payment
{
    public record GetAllPaymentQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<PaymentResponseDto>>>;

    public class GetAllPaymentQueryHandler(IPaymentService paymentService)
        : IRequestHandler<GetAllPaymentQuery, PagedResponse<IEnumerable<PaymentResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<PaymentResponseDto>>> Handle(GetAllPaymentQuery request, CancellationToken cancellationToken)
        {
            return await paymentService.GetPaymentsAsync(request.PageSize, request.PageNumber);
        }
    }
}

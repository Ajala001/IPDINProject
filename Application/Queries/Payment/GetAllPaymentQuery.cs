using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Queries.Payment
{
    public record GetAllPaymentQuery() : IRequest<ApiResponse<IEnumerable<PaymentResponseDto>>>;

    public class GetAllPaymentQueryHandler(IPaymentService paymentService)
        : IRequestHandler<GetAllPaymentQuery, ApiResponse<IEnumerable<PaymentResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<PaymentResponseDto>>> Handle(GetAllPaymentQuery request, CancellationToken cancellationToken)
        {
            return await paymentService.GetPaymentsAsync();
        }
    }
}

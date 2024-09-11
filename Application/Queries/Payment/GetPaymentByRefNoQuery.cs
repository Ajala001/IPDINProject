using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Queries.Payment
{
    public record GetPaymentByRefNoQuery(string ReferenceNo) : IRequest<ApiResponse<PaymentResponseDto>>;

    public class GetPaymentByRefNoQueryHandler(IPaymentService paymentService)
        : IRequestHandler<GetPaymentByRefNoQuery, ApiResponse<PaymentResponseDto>>
    {
        public async Task<ApiResponse<PaymentResponseDto>> Handle(GetPaymentByRefNoQuery request, CancellationToken cancellationToken)
        {
            return await paymentService.GetPaymentAsync(request.ReferenceNo);
        }
    }
}

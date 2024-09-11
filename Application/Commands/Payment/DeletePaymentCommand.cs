using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Payment
{
    public record DeletePaymentCommand(string ReferenceNo) : IRequest<ApiResponse<PaymentResponseDto>>;

    public class DeletePaymentCommandHandler(IPaymentService paymentService)
        : IRequestHandler<DeletePaymentCommand, ApiResponse<PaymentResponseDto>>
    {
        public async Task<ApiResponse<PaymentResponseDto>> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            return await paymentService.DeleteAsync(request.ReferenceNo);
        }
    }
}

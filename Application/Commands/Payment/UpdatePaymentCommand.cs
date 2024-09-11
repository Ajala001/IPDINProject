using App.Application.IExternalServices;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Payment
{
    public record UpdatePaymentCommand(string ReferenceNo, UpdatePaymentRequestDto UpdateRequest) 
        : IRequest<ApiResponse<PaymentResponseDto>>;

    public class UpdatePaymentCommandHandler(IPaymentService paymentService)
        : IRequestHandler<UpdatePaymentCommand, ApiResponse<PaymentResponseDto>>
    {
        public async Task<ApiResponse<PaymentResponseDto>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            return await paymentService.UpdateAsync(request.ReferenceNo, request.UpdateRequest);
        }
    }
}

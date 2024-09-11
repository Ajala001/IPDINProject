using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Payment
{
    public record VerifyPaymentCommand(string ReferenceNo) : IRequest<ApiResponse<string>>;

    public class VerifyPaymentCommandHandler(IPaymentService paymentService) 
        : IRequestHandler<VerifyPaymentCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
        {
            return await paymentService.VerifyPaymentAsync(request.ReferenceNo);
        }
    }
}

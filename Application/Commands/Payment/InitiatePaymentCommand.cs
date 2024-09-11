using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Payment
{
    public record InitiatePaymentCommand(CreatePaymentRequestDto CreateRequest) 
        : IRequest<ApiResponse<string>>;

    public class InitiatePaymentCommandHandler(IPaymentService paymentService)
        : IRequestHandler<InitiatePaymentCommand, ApiResponse<string>>
    {
        public async Task<ApiResponse<string>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
        {
            return await paymentService.InitiatePaymentAsync(request.CreateRequest);
        }
    }
}

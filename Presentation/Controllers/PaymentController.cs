using App.Application.Commands.Payment;
using App.Application.Queries.Course;
using App.Application.Queries.Payment;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Presentation.Controllers
{
    [Authorize(Roles = "Admin, Member")]
    [Route("api/payments")]
    [ApiController]
    public class PaymentController(ISender sender, ITokenService tokenService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPaymentAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetAllPaymentQuery(pageSize, pageNumber));
            if (result.IsSuccessful) return Ok(result);
            return NotFound(result);
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetUserPaymentsAsync([FromQuery] int pageSize, [FromQuery] int pageNumber)
        {
            var result = await sender.Send(new GetUserPaymentsQuery(pageSize, pageNumber));
            if (result.IsSuccessful) return Ok(result);
            return NotFound(result);
        }


        [HttpPost("initiatePayment")]
        public async Task<IActionResult> InitiatePaymentAsync([FromBody] CreatePaymentRequestDto request)
        {
            var result = await sender.Send(new InitiatePaymentCommand(request));
            if(result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpGet("verify")]
        public async Task<IActionResult> VerifyPaymentAsync([FromQuery] string reference)
        {
            var result = await sender.Send(new VerifyPaymentCommand(reference));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchForPaymentAsync([FromQuery] SearchQueryRequestDto searchRequestDto)
        {
            var result = await sender.Send(new SearchPaymentQuery(searchRequestDto));
            if (!result.IsSuccessful) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{referenceNo}")]
        public async Task<IActionResult> GetPaymentByReferenceNoAsync([FromRoute] string referenceNo)
        {
            var result = await sender.Send(new GetPaymentByRefNoQuery(referenceNo));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetPaymentDetails([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token) || !tokenService.ValidateToken(token, out string refNo))
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            var result = await sender.Send(new GetPaymentByRefNoQuery(refNo));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{referenceNo}")]
        public async Task<IActionResult> DeletePaymentAsync([FromRoute] string referenceNo)
        {
            var result = await sender.Send(new DeletePaymentCommand(referenceNo));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }


        [HttpPut("{referenceNo}")]
        public async Task<IActionResult> UpdatePaymentAsync([FromRoute] string referenceNo, [FromBody] UpdatePaymentRequestDto updateRequest)
        {
            var result = await sender.Send(new UpdatePaymentCommand(referenceNo, updateRequest));
            if (result.IsSuccessful) return Ok(result);
            return BadRequest(result);
        }

    }
}

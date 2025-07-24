using App.Core.Entities;
using App.Core.Enums;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text.Json;



namespace App.Application.AuthPolicy
{
    public class PaymentHandler(
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService,
        IConfiguration configuration) : AuthorizationHandler<PaymentRequirement>
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _configuration = configuration;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PaymentRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (!context.User.Identity?.IsAuthenticated ?? false)
            {
                context.Fail();
                return;
            }

            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email))
            {
                context.Fail();
                return;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                context.Fail();
                return;
            }

            if (user.HasPaidDues)
            {
                context.Succeed(requirement);
                return;
            }

            if (httpContext != null && !httpContext.Response.HasStarted)
            {

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                httpContext.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(new
                {
                    message = "Access denied: Dues unpaid. Please proceed to payment.",
                    code = "DuesNotPaid",
                    paymentData = new
                    {
                        serviceId = Guid.Empty.ToString(),
                        paymentType = "Dues" 
                    }
                });

                await httpContext.Response.WriteAsync(json);
            }

            context.Fail();
        }
    }
}

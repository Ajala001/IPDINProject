using App.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Application.AuthPolicy
{
    public class PaymentHandler(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor
        ) : AuthorizationHandler<PaymentRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PaymentRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            // Retrieve user's email from claims
            var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                context.Fail();
                return;
            }

            // Fetch user from the UserManager
            var user = await userManager.FindByEmailAsync(userEmail);

            // Check if the user has paid dues
            if (user != null && user.HasPaidDues)
            {
                context.Succeed(requirement); // Grant access if dues are paid
            }
            else
            {
                // Access HttpContext and return a 403 Forbidden response if dues are not paid
                var httpContext = httpContextAccessor.HttpContext;

                if (httpContext != null)
                {
                    // Set status code to 403
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await httpContext.Response.WriteAsync("User has not paid their dues.");
                }

                context.Fail(); // Deny access if dues are not paid
            }
        }
    }
}

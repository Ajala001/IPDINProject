using App.Core.Entities;
using App.Core.Enums;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Application.Services
{
    public class TokenService(IConfiguration configuration, UserManager<User> userManager,
        ILevelRepository levelRepository) : ITokenService
    {
        public async Task<string> GenerateAccessToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:AccessTokenExpirationMinutes"]!));

            var roles = await userManager.GetRolesAsync(user);
            var userLevel = await levelRepository.GetLevelAsync(l => l.Id == user.LevelId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("Surname", user.LastName),
                new Claim("GivenName", user.FirstName),
                new Claim("NameIdentifier", user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("Level", userLevel.Name),
                new Claim("MembershipNum", user.MembershipNumber!)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credential);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token, out string refNo)
        {
            refNo = null;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                refNo = principal.Claims.FirstOrDefault(c => c.Type == "refNo")?.Value!;
                return !string.IsNullOrEmpty(refNo);
            }
            catch
            {
                return false;
            }
        }


        public string GeneratePaymentToken(User user, Guid serviceId, PaymentType paymentType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("NameIdentifier", user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("MembershipNum", user.MembershipNumber!),
                new Claim("ServiceId", serviceId.ToString()),
                new Claim("PaymentType", paymentType.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = configuration["Jwt:ValidIssuer"],
                Audience = configuration["Jwt:ValidAudience"],
                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        public (bool IsValid, string Message, string? ServiceId, string? PaymentType) ValidatePaymentToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var serviceId = principal.Claims.FirstOrDefault(c => c.Type == "ServiceId")?.Value;
                var paymentType = principal.Claims.FirstOrDefault(c => c.Type == "PaymentType")?.Value;

                if (string.IsNullOrEmpty(serviceId) || string.IsNullOrEmpty(paymentType))
                {
                    return (false, "Token is missing required claims.", null, null);
                }

                return (true, "Token valid. Proceed to payment.", serviceId, paymentType);
            }
            catch (SecurityTokenException ex)
            {
                return (false, $"Token validation failed: {ex.Message}", null, null);
            }
            catch (Exception)
            {
                return (false, "An error occurred while validating the token.", null, null);
            }
        }

        public (bool IsValid, string? Email) ValidateUserToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
                var tokenHandler = new JwtSecurityTokenHandler();     

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:ValidAudience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                return (true, email);
            }
            catch
            {
                return (false, null);
            }
        }

        public string GeneratePaymentToken(string referenceNo)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("refNo", referenceNo)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), // Token valid for 30 minutes
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}

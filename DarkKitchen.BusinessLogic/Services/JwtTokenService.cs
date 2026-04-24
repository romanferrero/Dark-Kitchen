using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DarkKitchen.BusinessLogic.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private readonly byte[] keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Key"));

    public (int UserId, UserRole Role)? ValidateToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(keyBytes);
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
            }, out _);

            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;

            if(idClaim == null || roleClaim == null)
            {
                return null;
            }

            if(!int.TryParse(idClaim, out var userId))
            {
                return null;
            }

            if(!Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                return null;
            }

            return (userId, role);
        }
        catch
        {
            return null;
        }
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

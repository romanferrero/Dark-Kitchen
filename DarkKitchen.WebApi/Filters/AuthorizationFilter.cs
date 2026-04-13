using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizationFilter(params UserRole[] allowedRoles) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new ObjectResult("Authorization header is required") { StatusCode = 401 };
            return;
        }

        var token = authHeader["Bearer ".Length..];
        var jwtService = context.HttpContext.RequestServices.GetRequiredService<IJwtTokenService>();
        var claims = jwtService.ValidateToken(token);

        if (claims == null)
        {
            context.Result = new ObjectResult("Invalid token") { StatusCode = 401 };
            return;
        }

        if (allowedRoles.Length > 0 && !allowedRoles.Contains(claims.Value.Role))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
        }
    }
}

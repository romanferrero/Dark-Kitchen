using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizationFilterAttribute(params Permission[] requiredPermissions) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Result = new ObjectResult("Authorization header is required") { StatusCode = 401 };
            return;
        }

        var token = authHeader["Bearer ".Length..];
        var jwtService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
        var claims = jwtService.ValidateToken(token);

        if(claims == null)
        {
            context.Result = new ObjectResult("Invalid token") { StatusCode = 401 };
            return;
        }

        if(requiredPermissions.Length > 0 &&
           !requiredPermissions.Any(p => RolePermissions.RoleHas(claims.Value.Role, p)))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
            return;
        }

        context.HttpContext.Items["UserId"] = claims.Value.UserId;
        context.HttpContext.Items["UserRole"] = claims.Value.Role.ToString();
    }
}

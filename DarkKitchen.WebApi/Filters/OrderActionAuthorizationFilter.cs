using DarkKitchen.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method)]
public sealed class OrderActionAuthorizationFilter : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var roleString = context.HttpContext.Items["UserRole"]?.ToString();
        if (!Enum.TryParse<UserRole>(roleString, out _))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
        }
    }
}

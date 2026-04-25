using System.Text.Json;
using DarkKitchen.Domain.Enums;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
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
            return;
        }

        context.HttpContext.Request.EnableBuffering();
        var body = JsonSerializer.Deserialize<UpdateOrderStatusRequestModel>(
            context.HttpContext.Request.Body);
        context.HttpContext.Request.Body.Position = 0;

        if (body == null)
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
        }
    }
}

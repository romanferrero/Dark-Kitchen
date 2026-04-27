using System.Text.Json;
using DarkKitchen.Domain.Enums;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method)]
public sealed class OrderActionAuthorizationFilter : Attribute, IAuthorizationFilter
{
    private static readonly Dictionary<string, UserRole[]> _policies = new()
    {
        { "Prepared", [UserRole.Dispatcher, UserRole.Admin] },
        { "Cancelled", [UserRole.Admin] },
        { "OnTheWay", [UserRole.Dispatcher] },
        { "Delivered", [UserRole.Dispatcher] },
        { "NotDelivered", [UserRole.Dispatcher] },
    };

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var roleString = context.HttpContext.Items["UserRole"]?.ToString();
        if(!Enum.TryParse<UserRole>(roleString, out var role))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
            return;
        }

        context.HttpContext.Request.EnableBuffering();

        UpdateOrderStatusRequestModel? body = null;
        try
        {
            body = JsonSerializer.Deserialize<UpdateOrderStatusRequestModel>(
                context.HttpContext.Request.Body);
        }
        catch(JsonException)
        {
        }

        context.HttpContext.Request.Body.Position = 0;

        if(body == null || !_policies.TryGetValue(body.Action, out var allowed) || !allowed.Contains(role))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
        }
    }
}

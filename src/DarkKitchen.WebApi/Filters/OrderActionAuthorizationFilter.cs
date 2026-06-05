using System.Text.Json;
using DarkKitchen.Domain.Enums;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Method)]
public sealed class OrderActionAuthorizationFilter : Attribute, IAsyncAuthorizationFilter
{
    private static readonly Dictionary<string, Permission> ActionPermissions = new()
    {
        { "Prepared", Permission.PrepareOrder },
        { "Cancelled", Permission.CancelOrder },
        { "Delayed", Permission.PrepareOrder },
        { "OnTheWay", Permission.MoveOrderOnTheWay },
        { "Delivered", Permission.MarkOrderDelivered },
        { "NotDelivered", Permission.MarkOrderNotDelivered },
    };

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var roleString = context.HttpContext.Items["UserRole"]?.ToString();

        if(!Enum.TryParse<UserRole>(roleString, out var role))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
            return;
        }

        var request = context.HttpContext.Request;

        request.EnableBuffering();

        string bodyString;

        using(var reader = new StreamReader(request.Body, leaveOpen: true))
        {
            bodyString = await reader.ReadToEndAsync();
        }

        request.Body.Position = 0;

        UpdateOrderStatusRequestModel? body;

        try
        {
            body = JsonSerializer.Deserialize<UpdateOrderStatusRequestModel>(
                bodyString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch(JsonException)
        {
            context.Result = new ObjectResult("Invalid body") { StatusCode = 400 };
            return;
        }

        if(body == null ||
            string.IsNullOrWhiteSpace(body.Action) ||
            !ActionPermissions.TryGetValue(body.Action, out var requiredPermission) ||
            !RolePermissions.RoleHas(role, requiredPermission))
        {
            context.Result = new ObjectResult("Forbidden") { StatusCode = 403 };
        }
    }
}

using System.Text;
using System.Text.Json;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Test.FiltersTests;

[TestClass]
public class OrderActionAuthorizationFilterTests
{
    private AuthorizationFilterContext BuildContext(string? userRole, string? action)
    {
        var httpContext = new DefaultHttpContext();

        if(userRole != null)
        {
            httpContext.Items["UserRole"] = userRole;
        }

        if(action != null)
        {
            var json = JsonSerializer.Serialize(
                new UpdateOrderStatusRequestModel { Action = action });

            var bytes = Encoding.UTF8.GetBytes(json);

            httpContext.Request.Body = new MemoryStream(bytes);
            httpContext.Request.ContentType = "application/json";
        }
        else
        {
            httpContext.Request.Body = new MemoryStream();
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }

    [TestMethod]
    public async Task OnAuthorization_NoRole_Returns403()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext(null, "Prepared");

        await filter.OnAuthorizationAsync(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public async Task OnAuthorization_InvalidRole_Returns403()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("SuperAdmin", "Prepared");

        await filter.OnAuthorizationAsync(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public async Task OnAuthorization_NullBody_Returns403()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Admin", null);

        await filter.OnAuthorizationAsync(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public async Task OnAuthorization_InvalidAction_Returns403()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Admin", "Delete");

        await filter.OnAuthorizationAsync(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public async Task OnAuthorization_DispatcherCancelsOrder_Returns403()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Dispatcher", "Cancelled");

        await filter.OnAuthorizationAsync(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public async Task OnAuthorization_DispatcherDelaysOrder_Allows()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Dispatcher", "Delayed");

        await filter.OnAuthorizationAsync(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public async Task OnAuthorization_AdminDelaysOrder_Allows()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Admin", "Delayed");

        await filter.OnAuthorizationAsync(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public async Task OnAuthorization_ClientDelaysOrder_Returns403()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Client", "Delayed");

        await filter.OnAuthorizationAsync(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public async Task OnAuthorization_AfterExecution_BodyPositionIsReset()
    {
        var filter = new OrderActionAuthorizationFilterAttribute();
        var context = BuildContext("Admin", "Prepared");

        await filter.OnAuthorizationAsync(context);

        Assert.AreEqual(0, context.HttpContext.Request.Body.Position);
    }
}

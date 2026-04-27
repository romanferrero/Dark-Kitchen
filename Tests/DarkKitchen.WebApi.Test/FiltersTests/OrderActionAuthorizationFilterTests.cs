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
            var bytes = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(new UpdateOrderStatusRequestModel { Action = action }));
            httpContext.Request.Body = new MemoryStream(bytes);
            httpContext.Request.ContentType = "application/json";
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }

    [TestMethod]
    public void OnAuthorization_NoRole_Returns403()
    {
        var filter = new OrderActionAuthorizationFilter();
        var context = BuildContext(null, "Prepared");

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_InvalidRole_Returns403()
    {
        var filter = new OrderActionAuthorizationFilter();
        var context = BuildContext("SuperAdmin", "Prepared");

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_NullBody_Returns403()
    {
        var filter = new OrderActionAuthorizationFilter();
        var context = BuildContext("Admin", null);

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_InvalidAction_Returns403()
    {
        var filter = new OrderActionAuthorizationFilter();
        var context = BuildContext("Admin", "Delete");

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_DispatcherCancelsOrder_Returns403()
    {
        var filter = new OrderActionAuthorizationFilter();
        var context = BuildContext("Dispatcher", "Cancelled");

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(403, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_AfterExecution_BodyPositionIsReset()
    {
        var filter = new OrderActionAuthorizationFilter();
        var context = BuildContext("Admin", "Prepared");

        filter.OnAuthorization(context);

        Assert.AreEqual(0, context.HttpContext.Request.Body.Position);
    }
}

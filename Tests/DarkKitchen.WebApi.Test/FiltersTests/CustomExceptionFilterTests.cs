using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Test.FiltersTests;

[TestClass]
public class CustomExceptionFilterTests
{
    private CustomExceptionFilter _filter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _filter = new CustomExceptionFilter();
    }

    private static ExceptionContext BuildContext(Exception exception)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());
        return new ExceptionContext(actionContext, []) { Exception = exception };
    }

    [TestMethod]
    public void OnException_ArgumentException_Returns400()
    {
        var context = BuildContext(new ArgumentException("bad input"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void OnException_UnauthorizedAccessException_Returns401()
    {
        var context = BuildContext(new UnauthorizedAccessException("unauthorized"));

        _filter.OnException(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }
}

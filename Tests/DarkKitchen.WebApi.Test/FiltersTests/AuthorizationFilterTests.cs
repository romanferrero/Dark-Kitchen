using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DarkKitchen.WebApi.Test.FiltersTests;

[TestClass]
public class AuthorizationFilterTests
{
    private Mock<IJwtTokenService> _jwtServiceMock = null!;

    [TestInitialize]
    public void Initialize()
    {
        _jwtServiceMock = new Mock<IJwtTokenService>();
    }

    private AuthorizationFilterContext BuildContext(string? authHeader)
    {
        var httpContext = new DefaultHttpContext();

        if(authHeader != null)
        {
            httpContext.Request.Headers["Authorization"] = authHeader;
        }

        var services = new ServiceCollection();
        services.AddSingleton(_jwtServiceMock.Object);
        httpContext.RequestServices = services.BuildServiceProvider();

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }

    [TestMethod]
    public void OnAuthorization_NoHeader_Returns401()
    {
        var filter = new AuthorizationFilter();
        var context = BuildContext(null);

        filter.OnAuthorization(context);

        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(401, result.StatusCode);
    }
}

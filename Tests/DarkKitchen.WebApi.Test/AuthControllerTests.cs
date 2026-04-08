using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private AuthController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [TestMethod]
    public void Login_ValidCredentials_Returns200WithToken()
    {
        var request = new LoginRequestModel { Email = "user@test.com", Password = "ValidPass@1Ab!" };
        _authServiceMock
            .Setup(s => s.Login(request.Email, request.Password))
            .Returns("fake-token");

        var result = _controller.Login(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("fake-token", result.Value);
    }
}

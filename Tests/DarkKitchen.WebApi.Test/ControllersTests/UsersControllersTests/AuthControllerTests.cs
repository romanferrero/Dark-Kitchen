using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.UsersControllers;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.UsersControllersTests;

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
    public void Login_ValidCredentials_Returns201WithToken()
    {
        var request = new LoginRequestModel { Email = "user@test.com", Password = "ValidPass@1Ab!" };
        _authServiceMock
            .Setup(s => s.Login(request.Email, request.Password))
            .Returns("fake-token");

        var result = _controller.Login(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual("fake-token", result.Value);
    }

    [TestMethod]
    public void Login_InvalidCredentials_Returns401()
    {
        var request = new LoginRequestModel { Email = "user@test.com", Password = "WrongPass" };
        _authServiceMock
            .Setup(s => s.Login(request.Email, request.Password))
            .Throws(new UnauthorizedAccessException("Credenciales inválidas"));

        Assert.ThrowsException<UnauthorizedAccessException>(() => _controller.Login(request));
    }
}

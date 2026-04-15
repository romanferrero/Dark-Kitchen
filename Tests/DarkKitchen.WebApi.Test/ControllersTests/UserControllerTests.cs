using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserService> _clientServiceMock = null!;
    private UserController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _clientServiceMock = new Mock<IUserService>();
        _controller = new UserController(_clientServiceMock.Object);
    }

    [TestMethod]
    public void RegisterClient_InvalidData_Returns400()
    {
        _clientServiceMock
            .Setup(s => s.RegisterClient(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new ArgumentException("First name cannot be empty."));

        var request = new RegisterClientRequestModel
        {
            FirstName = string.Empty,
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass@1Ab!xyz",
        };

        var result = _controller.RegisterClient(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void RegisterClient_ValidData_Returns201()
    {
        var request = new RegisterClientRequestModel
        {
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass@1Ab!xyz",
        };

        var result = _controller.RegisterClient(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }
}

using DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.UsersControllers;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.UsersControllersTests;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userServiceMock = new Mock<IUserService>(MockBehavior.Strict);
        _controller = new UserController(_userServiceMock.Object);
    }

    [TestMethod]
    public void RegisterClient_InvalidData_ThrowsArgumentException()
    {
        _userServiceMock
            .Setup(s => s.RegisterClient(It.IsAny<RegisterClientEntryDto>()))
            .Throws(new ArgumentException("First name cannot be empty."));

        var request = new RegisterClientRequestModel
        {
            FirstName = string.Empty,
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass@1Ab!xyz",
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _controller.RegisterClient(request));
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

        var dto = new RegisterClientExitDto(
            "Juan",
            "Garcia",
            "juan@test.com",
            "099123456");

        _userServiceMock
            .Setup(s => s.RegisterClient(It.IsAny<RegisterClientEntryDto>()))
            .Returns(dto);

        var result = _controller.RegisterClient(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }
}

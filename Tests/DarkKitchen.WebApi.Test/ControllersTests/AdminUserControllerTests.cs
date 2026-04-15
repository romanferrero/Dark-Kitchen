using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class AdminUserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private AdminUserController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new AdminUserController(_userServiceMock.Object);
    }

    [TestMethod]
    public void CreateUser_ValidData_Returns201()
    {
        var request = new CreateUserRequestModel
        {
            FirstName = "Pedro",
            LastName = "Lopez",
            Email = "pedro@test.com",
            Phone = "099654321",
            Password = "ValidPass@1Ab!xyz",
            Role = "Admin",
        };

        var result = _controller.CreateUser(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }
}

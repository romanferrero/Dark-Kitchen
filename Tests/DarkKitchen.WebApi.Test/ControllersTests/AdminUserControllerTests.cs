using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Filters;
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

    private static CreateUserRequestModel CreateValidRequest()
    {
        return new CreateUserRequestModel
        {
            FirstName = "Pedro",
            LastName = "Lopez",
            Email = "pedro@test.com",
            Phone = "099654321",
            Password = "ValidPass@1Ab!xyz",
            Role = "Admin",
        };
    }

    [TestMethod]
    public void CreateUser_ValidData_Returns201()
    {
        var request = CreateValidRequest();

        var result = _controller.CreateUser(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }

    [TestMethod]
    public void CreateUser_ValidData_CallsServiceWithSameData()
    {
        var request = CreateValidRequest();

        _controller.CreateUser(request);

        _userServiceMock.Verify(s => s.CreateUser(
            "Pedro",
            "Lopez",
            "pedro@test.com",
            "099654321",
            "ValidPass@1Ab!xyz",
            "Admin"), Times.Once);
    }

    [TestMethod]
    public void CreateUser_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminUserController).GetMethod("CreateUser");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }

    [TestMethod]
    public void DeleteUser_ValidId_Returns200()
    {
        var result = _controller.DeleteUser(5);

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void DeleteUser_ValidId_CallsServiceWithSameId()
    {
        _controller.DeleteUser(5);

        _userServiceMock.Verify(s => s.DeleteUser(5, 0), Times.Once);
    }

    [TestMethod]
    public void DeleteUser_HasHttpDeleteAttribute()
    {
        var method = typeof(AdminUserController).GetMethod("DeleteUser");

        var attributes = method!.GetCustomAttributes(typeof(HttpDeleteAttribute), false);

        Assert.AreEqual(1, attributes.Length);
    }

    [TestMethod]
    public void DeleteUser_HasHttpDeleteRouteWithId()
    {
        var method = typeof(AdminUserController).GetMethod("DeleteUser");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpDeleteAttribute), false)
            .Cast<HttpDeleteAttribute>()
            .Single();

        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void DeleteUser_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminUserController).GetMethod("DeleteUser");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }
}

using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
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

    private static UpdateUserRequestModel CreateValidUpdateRequest()
    {
        return new UpdateUserRequestModel
        {
            FirstName = "Pedro",
            LastName = "Lopez",
            Email = "pedro@test.com",
            Phone = "099654321",
            Password = "ValidPass@1Ab!xyz",
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

    [TestMethod]
    public void UpdateUser_ValidData_Returns200()
    {
        var request = CreateValidUpdateRequest();

        var result = _controller.UpdateUser(5, request);

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void UpdateUser_ValidData_CallsServiceWithSameData()
    {
        var request = CreateValidUpdateRequest();

        _controller.UpdateUser(5, request);

        _userServiceMock.Verify(s => s.UpdateUser(
            5,
            "Pedro",
            "Lopez",
            "pedro@test.com",
            "099654321",
            "ValidPass@1Ab!xyz",
            0), Times.Once);
    }

    [TestMethod]
    public void UpdateUser_HasHttpPutAttribute()
    {
        var method = typeof(AdminUserController).GetMethod("UpdateUser");

        var attributes = method!.GetCustomAttributes(typeof(HttpPutAttribute), false);

        Assert.AreEqual(1, attributes.Length);
    }

    [TestMethod]
    public void UpdateUser_HasHttpPutRouteWithId()
    {
        var method = typeof(AdminUserController).GetMethod("UpdateUser");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpPutAttribute), false)
            .Cast<HttpPutAttribute>()
            .Single();

        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void UpdateUser_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminUserController).GetMethod("UpdateUser");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }

    [TestMethod]
    public void GetUsers_NoFilters_Returns200()
    {
        var result = _controller.GetUsers();

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public void GetUsers_NoFilters_CallsServiceWithNullFilters()
    {
        _userServiceMock
            .Setup(s => s.GetUsers(null, null))
            .Returns(new List<GetUsersDto>());

        _controller.GetUsers();

        _userServiceMock.Verify(s => s.GetUsers(null, null), Times.Once);
    }

    [TestMethod]
    public void GetUsers_WithFilters_CallsServiceWithSameFilters()
    {
        _userServiceMock
            .Setup(s => s.GetUsers("Pedro", "Lopez"))
            .Returns(new List<GetUsersDto>());

        _controller.GetUsers("Pedro", "Lopez");

        _userServiceMock.Verify(s => s.GetUsers("Pedro", "Lopez"), Times.Once);
    }

    [TestMethod]
    public void GetUsers_HasHttpGetAttribute()
    {
        var method = typeof(AdminUserController).GetMethod("GetUsers");

        var attributes = method!.GetCustomAttributes(typeof(HttpGetAttribute), false);

        Assert.AreEqual(1, attributes.Length);
    }
}

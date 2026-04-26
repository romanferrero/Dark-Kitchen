using DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.AdminControllers;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.AdminControllersTests;

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

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = 1;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
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

    private static UserExitDto CreateUserExitDto(int id = 1)
    {
        return new UserExitDto
        {
            Id = id,
            FirstName = "Pedro",
            LastName = "Lopez",
            Email = "pedro@test.com",
            Phone = "099654321",
            Role = "Admin"
        };
    }

    [TestMethod]
    public void CreateUser_ValidData_Returns201()
    {
        _userServiceMock
            .Setup(s => s.CreateUser(It.IsAny<CreateUserEntryDto>()))
            .Returns(CreateUserExitDto());

        var result = _controller.CreateUser(CreateValidRequest());

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }

    [TestMethod]
    public void CreateUser_ValidData_CallsServiceWithDto()
    {
        _userServiceMock
            .Setup(s => s.CreateUser(It.IsAny<CreateUserEntryDto>()))
            .Returns(CreateUserExitDto());

        _controller.CreateUser(CreateValidRequest());

        _userServiceMock.Verify(s => s.CreateUser(
            It.Is<CreateUserEntryDto>(dto =>
                dto.FirstName == "Pedro" &&
                dto.LastName == "Lopez" &&
                dto.Email == "pedro@test.com" &&
                dto.Phone == "099654321" &&
                dto.Password == "ValidPass@1Ab!xyz" &&
                dto.Role == "Admin")), Times.Once);
    }

    [TestMethod]
    public void CreateUser_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminUserController).GetMethod("CreateUser");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }

    [TestMethod]
    public void DeleteUser_ValidId_ReturnsNoContent()
    {
        var result = _controller.DeleteUser(5);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void DeleteUser_ValidId_CallsServiceWithSameId()
    {
        _controller.DeleteUser(5);

        _userServiceMock.Verify(s => s.DeleteUser(5, 1), Times.Once);
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
        _userServiceMock
            .Setup(s => s.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserEntryDto>(), It.IsAny<int>()))
            .Returns(CreateUserExitDto(5));

        var result = _controller.UpdateUser(5, CreateValidUpdateRequest());

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateUser_ValidData_CallsServiceWithDto()
    {
        _userServiceMock
            .Setup(s => s.UpdateUser(It.IsAny<int>(), It.IsAny<UpdateUserEntryDto>(), It.IsAny<int>()))
            .Returns(CreateUserExitDto(5));

        _controller.UpdateUser(5, CreateValidUpdateRequest());

        _userServiceMock.Verify(s => s.UpdateUser(
            5,
            It.Is<UpdateUserEntryDto>(dto =>
                dto.FirstName == "Pedro" &&
                dto.LastName == "Lopez" &&
                dto.Email == "pedro@test.com" &&
                dto.Phone == "099654321" &&
                dto.Password == "ValidPass@1Ab!xyz"),
            1), Times.Once);
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
        _userServiceMock
            .Setup(s => s.GetUsers(null, null))
            .Returns([]);

        var result = _controller.GetUsers() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetUsers_NoFilters_CallsServiceWithNullFilters()
    {
        _userServiceMock
            .Setup(s => s.GetUsers(null, null))
            .Returns([]);

        _controller.GetUsers();

        _userServiceMock.Verify(s => s.GetUsers(null, null), Times.Once);
    }

    [TestMethod]
    public void GetUsers_WithFilters_CallsServiceWithSameFilters()
    {
        _userServiceMock
            .Setup(s => s.GetUsers("Pedro", "Lopez"))
            .Returns([]);

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

    [TestMethod]
    public void GetUsers_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminUserController).GetMethod("GetUsers");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }
}

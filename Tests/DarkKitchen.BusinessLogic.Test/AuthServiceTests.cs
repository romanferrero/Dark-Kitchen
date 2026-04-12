using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IJwtTokenService> _jwtTokenServiceMock = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _authService = new AuthService(_userRepositoryMock.Object, _jwtTokenServiceMock.Object);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnsNonEmptyToken()
    {
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!x" };
        _userRepositoryMock
            .Setup(r => r.GetByEmail("user@test.com"))
            .Returns(user);
        _jwtTokenServiceMock
            .Setup(s => s.GenerateToken(user))
            .Returns("jwt-token");

        var token = _authService.Login("user@test.com", "ValidPass@1Ab!x");

        Assert.IsNotNull(token);
        Assert.IsFalse(string.IsNullOrEmpty(token));
    }

    [TestMethod]
    public void Login_UserNotFound_ThrowsInvalidOperationException()
    {
        _userRepositoryMock
            .Setup(r => r.GetByEmail("noexiste@test.com"))
            .Returns((User?)null);

        Assert.ThrowsException<InvalidOperationException>(
            () => _authService.Login("noexiste@test.com", "cualquierpass"));
    }

    [TestMethod]
    public void Login_WrongPassword_ThrowsInvalidOperationException()
    {
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!x" };
        _userRepositoryMock
            .Setup(r => r.GetByEmail("user@test.com"))
            .Returns(user);

        Assert.ThrowsException<InvalidOperationException>(
            () => _authService.Login("user@test.com", "WrongPassword!1A"));
    }
}

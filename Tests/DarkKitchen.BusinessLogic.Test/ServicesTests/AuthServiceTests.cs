using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class AuthServiceTests
{
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private Mock<ITokenService> _jwtTokenServiceMock = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _jwtTokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict);
        _authService = new AuthService(_userRepositoryMock.Object, _jwtTokenServiceMock.Object);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnsNonEmptyToken()
    {
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!x" };
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(user);
        _jwtTokenServiceMock
            .Setup(s => s.GenerateToken(user))
            .Returns("jwt-token");

        var token = _authService.Login("user@test.com", "ValidPass@1Ab!x");

        Assert.IsNotNull(token);
        Assert.IsFalse(string.IsNullOrEmpty(token));
    }

    [TestMethod]
    public void Login_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        Assert.ThrowsException<UnauthorizedAccessException>(
            () => _authService.Login("noexiste@test.com", "cualquierpass"));
    }

    [TestMethod]
    public void Login_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!x" };
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(user);

        Assert.ThrowsException<UnauthorizedAccessException>(
            () => _authService.Login("user@test.com", "WrongPassword!1A"));
    }
}

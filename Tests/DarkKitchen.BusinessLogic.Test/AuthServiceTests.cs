using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private AuthService _authService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authService = new AuthService(_userRepositoryMock.Object);
    }

    [TestMethod]
    public void Login_ValidCredentials_ReturnsNonEmptyToken()
    {
        var user = new User { Email = "user@test.com", Password = "ValidPass@1Ab!" };
        _userRepositoryMock
            .Setup(r => r.GetByEmail("user@test.com"))
            .Returns(user);

        var token = _authService.Login("user@test.com", "ValidPass@1Ab!");

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
}

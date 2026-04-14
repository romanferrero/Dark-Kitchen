using DarkKitchen.Domain;
using DarkKitchen.WebApi.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DarkKitchen.WebApi.Test.ServicesTests;

[TestClass]
public class JwtTokenServiceTests
{
    private Mock<IConfiguration> _configMock = null!;
    private JwtTokenService _service = null!;

    private const string TestKey = "super-secret-key-for-testing-1234567890!!";

    [TestInitialize]
    public void Initialize()
    {
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Jwt:Key"]).Returns(TestKey);
        _service = new JwtTokenService(_configMock.Object);
    }

    private User BuildUser(UserRole role = UserRole.Admin) =>
        new()
        {
            Id = 42,
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            Phone = "099000000",
            Password = "ValidPass@1Ab!x",
            Role = role,
        };

    [TestMethod]
    public void ValidateToken_ValidToken_ReturnsClaims()
    {
        var user = BuildUser(UserRole.Admin);
        var token = _service.GenerateToken(user);

        var result = _service.ValidateToken(token);

        Assert.IsNotNull(result);
        Assert.AreEqual(42, result.Value.UserId);
        Assert.AreEqual(UserRole.Admin, result.Value.Role);
    }

    [TestMethod]
    public void ValidateToken_InvalidToken_ReturnsNull()
    {
        var result = _service.ValidateToken("not.a.valid.token");

        Assert.IsNull(result);
    }
}

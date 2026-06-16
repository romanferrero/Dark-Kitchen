using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace DarkKitchen.WebApi.Test.ServicesTests;

[TestClass]
public class TokenServiceTests
{
    private Mock<IConfiguration> _configMock = null!;
    private TokenService _service = null!;

    private const string TestKey = "super-secret-key-for-testing-1234567890!!";

    [TestInitialize]
    public void Initialize()
    {
        _configMock = new Mock<IConfiguration>(MockBehavior.Strict);
        _configMock.Setup(c => c["Jwt:Key"]).Returns(TestKey);
        _service = new TokenService(_configMock.Object);
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
        var user = BuildUser();
        var token = _service.GenerateToken(user);

        var result = _service.ValidateToken(token);

        Assert.IsNotNull(result);
        Assert.AreEqual(42, result.Value.UserId);
        Assert.AreEqual(UserRole.Admin, result.Value.Role);
    }

    [TestMethod]
    public void GenerateToken_IncludesPermissionClaimsForRole()
    {
        var user = BuildUser(UserRole.Client);
        var token = _service.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var permissions = jwt.Claims
            .Where(c => c.Type == "permissions")
            .Select(c => c.Value)
            .ToList();

        var expected = RolePermissions.PermissionsFor(UserRole.Client)
            .Select(p => p.ToString())
            .ToList();

        CollectionAssert.AreEquivalent(expected, permissions);
    }

    private string BuildTokenWithClaims(params Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [TestMethod]
    public void ValidateToken_InvalidToken_ReturnsNull()
    {
        var result = _service.ValidateToken("not.a.valid.token");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ValidateToken_TokenMissingIdClaim_ReturnsNull()
    {
        var token = BuildTokenWithClaims(
            new Claim(ClaimTypes.Role, "Admin"));

        var result = _service.ValidateToken(token);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ValidateToken_TokenMissingRoleClaim_ReturnsNull()
    {
        var token = BuildTokenWithClaims(
            new Claim(ClaimTypes.NameIdentifier, "42"));

        var result = _service.ValidateToken(token);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ValidateToken_TokenWithInvalidUserId_ReturnsNull()
    {
        var token = BuildTokenWithClaims(
            new Claim(ClaimTypes.NameIdentifier, "not-an-int"),
            new Claim(ClaimTypes.Role, "Admin"));

        var result = _service.ValidateToken(token);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ValidateToken_TokenWithInvalidRole_ReturnsNull()
    {
        var token = BuildTokenWithClaims(
            new Claim(ClaimTypes.NameIdentifier, "42"),
            new Claim(ClaimTypes.Role, "InvalidRole"));

        var result = _service.ValidateToken(token);

        Assert.IsNull(result);
    }
}

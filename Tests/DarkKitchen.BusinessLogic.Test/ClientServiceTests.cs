using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ClientServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private ClientService _clientService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _clientService = new ClientService(_userRepositoryMock.Object);
    }

    [TestMethod]
    public void RegisterClient_ApellidoTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Ga", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_ApellidoTooLong_ThrowsArgumentException()
    {
        var apellidoLargo = new string('A', 26);

        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", apellidoLargo, "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_EmptyNombre_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient(string.Empty, "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_InvalidEmail_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "emailinvalido", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_PasswordTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "Short@1A"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoUppercase_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "nouppercase@1abc"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoLowercase_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "NOLOWERCASE@1ABC"));
    }

    [TestMethod]
    public void RegisterClient_ValidData_CallsRepositoryAdd()
    {
        _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.Nombre == "Juan" &&
                u.Apellido == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Telefono == "099123456" &&
                u.Rol == UserRole.Client)),
            Times.Once);
    }
}

using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
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
    public void RegisterClient_EmptyFirstName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient(string.Empty, "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_LastNameTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Ga", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_LastNameTooLong_ThrowsArgumentException()
    {
        var longLastName = new string('A', 26);

        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", longLastName, "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
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
    public void RegisterClient_PasswordNoSymbol_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "NoSymbolPass1Abcd"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoDigit_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "NoDigitPass@Abcde"));
    }

    [TestMethod]
    public void RegisterClient_PasswordWithNumericSequence_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@A123bcd"));
    }

    [TestMethod]
    public void RegisterClient_ValidData_CallsRepositoryAdd()
    {
        _clientService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Role == UserRole.Client)),
            Times.Once);
    }
}

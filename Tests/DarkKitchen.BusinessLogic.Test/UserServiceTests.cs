using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class UserServiceTests
{
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private UserService _userService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [TestMethod]
    public void RegisterClient_EmptyFirstName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(string.Empty, "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_LastNameTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Ga", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_LastNameTooLong_ThrowsArgumentException()
    {
        var longLastName = new string('A', 26);

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", longLastName, "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_InvalidEmail_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "emailinvalido", "099123456", "ValidPass@1Ab!xyz"));
    }

    [TestMethod]
    public void RegisterClient_PasswordTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "Short@1A"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoUppercase_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "nouppercase@1abc"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoLowercase_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "NOLOWERCASE@1ABC"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoSymbol_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "NoSymbolPass1Abcd"));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoDigit_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "NoDigitPass@Abcde"));
    }

    [TestMethod]
    public void RegisterClient_PasswordWithNumericSequence_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@A123bcd"));
    }

    [TestMethod]
    public void RegisterClient_ValidData_CallsRepositoryAdd()
    {
        _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Role == UserRole.Client)),
            Times.Once);
    }

    [TestMethod]
    public void CreateUser_InvalidRole_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser(
                "Juan",
                "Garcia",
                "juan@test.com",
                "099123456",
                "ValidPass@1Ab!xyz",
                "Chef"));
    }

    [TestMethod]
    public void CreateUser_ValidAdmin_CallsRepositoryAdd()
    {
        _userService.CreateUser(
            "Juan",
            "Garcia",
            "juan@test.com",
            "099123456",
            "ValidPass@1Ab!xyz",
            "Admin");

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Role == UserRole.Admin)),
            Times.Once);
    }

    [TestMethod]
    public void CreateUser_ValidDispatcher_CallsRepositoryAdd()
    {
        _userService.CreateUser(
            "Juan",
            "Garcia",
            "juan@test.com",
            "099123456",
            "ValidPass@1Ab!xyz",
            "Dispatcher");

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.Role == UserRole.Dispatcher)),
            Times.Once);
    }

    [TestMethod]
    public void DeleteUser_SameAsCurrentUser_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.DeleteUser(5, 5));
    }

    [TestMethod]
    public void DeleteUser_UserDoesNotExist_ThrowsArgumentException()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<User>());

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.DeleteUser(5, 1));
    }

    [TestMethod]
    public void DeleteUser_ValidUser_CallsRepositoryDelete()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<User>
            {
                new User
                {
                    Id = 5,
                    FirstName = "Juan",
                    LastName = "Garcia",
                    Email = "juan@test.com",
                    Phone = "099123456",
                    Password = "ValidPass@1Ab!xyz",
                    Role = UserRole.Admin
                }
            });

        _userService.DeleteUser(5, 1);

        _userRepositoryMock.Verify(
            r => r.Delete(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()),
            Times.Once);
    }

    [TestMethod]
    public void UpdateUser_SameAsCurrentUser_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser(
                5,
                "Juan",
                "Garcia",
                "juan@test.com",
                "099123456",
                "ValidPass@1Ab!xyz",
                5));
    }

    [TestMethod]
    public void UpdateUser_UserDoesNotExist_ThrowsArgumentException()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<User>());

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser(
                5,
                "Juan",
                "Garcia",
                "juan@test.com",
                "099123456",
                "ValidPass@1Ab!xyz",
                1));
    }

    [TestMethod]
    public void UpdateUser_ValidData_CallsRepositoryUpdate()
    {
        var existingUser = new User
        {
            Id = 5,
            FirstName = "Viejo",
            LastName = "Nombre",
            Email = "viejo@test.com",
            Phone = "099111111",
            Password = "OldPassword@1Abc",
            Role = UserRole.Admin
        };

        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<User> { existingUser });

        _userService.UpdateUser(
            5,
            "Juan",
            "Garcia",
            "juan@test.com",
            "099123456",
            "ValidPass@1Ab!xyz",
            1);

        _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u =>
                u.Id == 5 &&
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Password == "ValidPass@1Ab!xyz")),
            Times.Once);
    }
}

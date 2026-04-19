using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class UserServiceTests
{
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private Mock<IPhoneValidator> _phoneValidatorMock = null!;
    private UserService _userService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([]);

        _phoneValidatorMock = new Mock<IPhoneValidator>();
        _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
        _phoneValidatorMock.Setup(v => v.ErrorMessage).Returns("Invalid phone number.");

        _userService = new UserService(_userRepositoryMock.Object, _phoneValidatorMock.Object);
    }

    private static User CreateUserEntity(
        int id,
        string firstName,
        string lastName,
        string email)
    {
        return new User
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = "099123456",
            Password = "ValidPass@1Ab!xyz",
            Role = UserRole.Admin
        };
    }

    private static List<User> CreateUsers()
    {
        return
        [
            CreateUserEntity(1, "Juan", "Garcia", "juan@test.com"),
            CreateUserEntity(2, "Pedro", "Lopez", "pedro@test.com"),
            CreateUserEntity(3, "Pedro", "Gomez", "pgomez@test.com")
        ];
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
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "Short@1Abcdefg"));
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
    public void RegisterClient_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([CreateUserEntity(1, "Existing", "User", "juan@test.com")]);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz"));
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
                u.Password == "ValidPass@1Ab!xyz" &&
                u.Role == UserRole.Client)),
            Times.Once);
    }

    [TestMethod]
    public void CreateUser_InvalidRole_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser(
                "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", "Chef"));
    }

    [TestMethod]
    public void CreateUser_ClientRole_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser(
                "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", "Client"));
    }

    [TestMethod]
    public void CreateUser_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([CreateUserEntity(1, "Existing", "User", "juan@test.com")]);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.CreateUser(
                "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", "Admin"));
    }

    [TestMethod]
    public void CreateUser_ValidAdmin_CallsRepositoryAdd()
    {
        _userService.CreateUser(
            "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", "Admin");

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
            "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", "Dispatcher");

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
    public void DeleteUser_UserDoesNotExist_ThrowsKeyNotFoundException()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([]);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _userService.DeleteUser(5, 1));
    }

    [TestMethod]
    public void DeleteUser_ValidUser_CallsRepositoryDelete()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([CreateUserEntity(5, "Juan", "Garcia", "juan@test.com")]);

        _userService.DeleteUser(5, 1);

        _userRepositoryMock.Verify(
            r => r.Delete(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()),
            Times.Once);
    }

    [TestMethod]
    public void UpdateUser_SameAsCurrentUser_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser(5, "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", 5));
    }

    [TestMethod]
    public void UpdateUser_UserDoesNotExist_ThrowsKeyNotFoundException()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([]);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _userService.UpdateUser(5, "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", 1));
    }

    [TestMethod]
    public void UpdateUser_ValidData_CallsRepositoryUpdate()
    {
        var existingUser = new User
        {
            Id = 5,
            FirstName = "Viejo",
            LastName = "Nombre",
            Email = "juan@test.com",
            Phone = "099111111",
            Password = "OldPassword@1Abc",
            Role = UserRole.Admin
        };

        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([existingUser]);

        _userService.UpdateUser(5, "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", 1);

        _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u =>
                u.Id == 5 &&
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Password == "ValidPass@1Ab!xyz")),
            Times.Once);
    }

    [TestMethod]
    public void UpdateUser_SameEmail_DoesNotValidateUniqueness()
    {
        var existingUser = new User
        {
            Id = 5,
            FirstName = "Viejo",
            LastName = "Nombre",
            Email = "juan@test.com",
            Phone = "099111111",
            Password = "OldPassword@1Abc",
            Role = UserRole.Admin
        };

        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([existingUser]);

        _userService.UpdateUser(5, "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", 1);

        _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u => u.Email == "juan@test.com")), Times.Once);
    }

    [TestMethod]
    public void UpdateUser_DifferentEmailAlreadyTaken_ThrowsInvalidOperationException()
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

        var callCount = 0;
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns(() =>
            {
                callCount++;
                if(callCount == 1)
                {
                    return [existingUser];
                }

                return [CreateUserEntity(10, "Otro", "Usuario", "taken@test.com")];
            });

        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.UpdateUser(5, "Juan", "Garcia", "taken@test.com", "099123456", "ValidPass@1Ab!xyz", 1));
    }

    [TestMethod]
    public void GetUsers_NoFilters_ReturnsAllMappedDtos()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(CreateUsers());

        var result = _userService.GetUsers(null, null);

        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Juan", result[0].FirstName);
        Assert.AreEqual("Garcia", result[0].LastName);
        Assert.AreEqual("juan@test.com", result[0].Email);
    }

    [TestMethod]
    public void GetUsers_WithFirstNameFilter_ReturnsMatchingUsers()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(CreateUsers());

        var result = _userService.GetUsers("Pedro", null);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(u => u.FirstName == "Pedro"));
    }

    [TestMethod]
    public void GetUsers_WithFirstNameFilterCaseInsensitive_ReturnsMatchingUsers()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(CreateUsers());

        var result = _userService.GetUsers("pedro", null);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(u => u.FirstName == "Pedro"));
    }

    [TestMethod]
    public void GetUsers_WithLastNameFilter_ReturnsMatchingUsers()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(CreateUsers());

        var result = _userService.GetUsers(null, "Lopez");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Lopez", result[0].LastName);
    }

    [TestMethod]
    public void GetUsers_WithBothFilters_ReturnsMatchingUsersOnly()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(CreateUsers());

        var result = _userService.GetUsers("Pedro", "Lopez");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pedro", result[0].FirstName);
        Assert.AreEqual("Lopez", result[0].LastName);
    }

    [TestMethod]
    public void GetUsers_WithNonMatchingFilter_ReturnsEmptyList()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(CreateUsers());

        var result = _userService.GetUsers("Inexistente", null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void RegisterClient_InvalidPhone_ThrowsArgumentException()
    {
        _phoneValidatorMock.Setup(v => v.IsValid("12345")).Returns(false);
        _phoneValidatorMock.Setup(v => v.ErrorMessage)
            .Returns("Phone must be a valid Uruguayan mobile number (09XXXXXXX).");

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient("Juan", "Garcia", "juan@test.com", "12345", "ValidPass@1Ab!xyz"));

        Assert.AreEqual("Phone must be a valid Uruguayan mobile number (09XXXXXXX).", ex.Message);
    }

    [TestMethod]
    public void CreateUser_InvalidPhone_ThrowsArgumentException()
    {
        _phoneValidatorMock.Setup(v => v.IsValid("12345")).Returns(false);
        _phoneValidatorMock.Setup(v => v.ErrorMessage)
            .Returns("Phone must be a valid Uruguayan mobile number (09XXXXXXX).");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser("Juan", "Garcia", "juan@test.com", "12345", "ValidPass@1Ab!xyz", "Admin"));
    }

    [TestMethod]
    public void UpdateUser_InvalidPhone_ThrowsArgumentException()
    {
        var existingUser = CreateUserEntity(5, "Viejo", "Nombre", "viejo@test.com");

        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .Returns([existingUser]);

        _phoneValidatorMock.Setup(v => v.IsValid("12345")).Returns(false);
        _phoneValidatorMock.Setup(v => v.ErrorMessage)
            .Returns("Phone must be a valid Uruguayan mobile number (09XXXXXXX).");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser(5, "Juan", "Garcia", "viejo@test.com", "12345", "ValidPass@1Ab!xyz", 1));
    }
}

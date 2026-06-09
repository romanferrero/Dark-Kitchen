using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IBusinessLogic.IValidators;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class UserServiceTests
{
    private const string HashedPassword = "hashed-password";

    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private Mock<IPhoneValidator> _phoneValidatorMock = null!;
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private UserService _userService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _userRepositoryMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns((User?)null);
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);
        _userRepositoryMock.Setup(r => r.Add(It.IsAny<User>()));
        _userRepositoryMock.Setup(r => r.Update(It.IsAny<User>()));
        _userRepositoryMock.Setup(r => r.Delete(It.IsAny<Expression<Func<User, bool>>>()));

        _phoneValidatorMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        _phoneValidatorMock.Setup(v => v.IsValid(It.IsAny<string>())).Returns(true);
        _phoneValidatorMock.Setup(v => v.ErrorMessage).Returns("Invalid phone number.");

        _passwordHasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
        _passwordHasherMock.Setup(h => h.Hash(It.IsAny<string>())).Returns(HashedPassword);

        _userService = new UserService(
            _userRepositoryMock.Object, _phoneValidatorMock.Object, _passwordHasherMock.Object);
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

    private static CreateUserEntryDto CreateValidUserDto(string role = "Admin")
    {
        return new CreateUserEntryDto(
            "Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz", role);
    }

    private static UpdateUserEntryDto CreateValidUpdateDto(
        string email = "juan@test.com",
        string phone = "099123456")
    {
        return new UpdateUserEntryDto(
            "Juan", "Garcia", email, phone, "ValidPass@1Ab!xyz");
    }

    [TestMethod]
    public void RegisterClient_EmptyFirstName_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto(string.Empty, "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_LastNameTooShort_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Ga", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_LastNameTooLong_ThrowsArgumentException()
    {
        var longLastName = new string('A', 26);
        var dto = new RegisterClientEntryDto("Juan", longLastName, "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_InvalidEmail_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "emailinvalido", "099123456", "ValidPass@1Ab!xyz");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_PasswordTooShort_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "Short@1Abcdefg");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoUppercase_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "nouppercase@1abc");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoLowercase_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "NOLOWERCASE@1ABC");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoSymbol_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "NoSymbolPass1Abcd");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_PasswordNoDigit_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "NoDigitPass@Abcde");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_PasswordWithNumericSequence_ThrowsArgumentException()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@A123bcd");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.RegisterClient(dto));
    }

    [TestMethod]
    public void RegisterClient_ValidData_CallsRepositoryAdd()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        _userService.RegisterClient(dto);

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Password == HashedPassword &&
                u.Role == UserRole.Client)),
            Times.Once);
    }

    [TestMethod]
    public void RegisterClient_ValidData_HashesRawPasswordBeforeStoring()
    {
        var dto = new RegisterClientEntryDto("Juan", "Garcia", "juan@test.com", "099123456", "ValidPass@1Ab!xyz");

        _userService.RegisterClient(dto);

        _passwordHasherMock.Verify(h => h.Hash("ValidPass@1Ab!xyz"), Times.Once);
    }

    [TestMethod]
    public void CreateUser_InvalidRole_ThrowsArgumentException()
    {
        var dto = CreateValidUserDto("Chef");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser(dto));
    }

    [TestMethod]
    public void CreateUser_ClientRole_ThrowsArgumentException()
    {
        var dto = CreateValidUserDto("Client");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser(dto));
    }

    [TestMethod]
    public void CreateUser_DuplicateEmail_ThrowsInvalidOperationException()
    {
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        var dto = CreateValidUserDto();

        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.CreateUser(dto));
    }

    [TestMethod]
    public void CreateUser_ValidAdmin_CallsRepositoryAdd()
    {
        var dto = CreateValidUserDto("Admin");

        _userService.CreateUser(dto);

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
        var dto = CreateValidUserDto("Dispatcher");

        _userService.CreateUser(dto);

        _userRepositoryMock.Verify(
            r => r.Add(It.Is<User>(u =>
                u.Role == UserRole.Dispatcher)),
            Times.Once);
    }

    [TestMethod]
    public void CreateUser_ValidData_ReturnsUserExitDto()
    {
        var dto = CreateValidUserDto();

        var result = _userService.CreateUser(dto);

        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Garcia", result.LastName);
        Assert.AreEqual("juan@test.com", result.Email);
        Assert.AreEqual("Admin", result.Role);
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
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _userService.DeleteUser(5, 1));
    }

    [TestMethod]
    public void DeleteUser_ValidUser_CallsRepositoryDelete()
    {
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(CreateUserEntity(5, "Juan", "Garcia", "juan@test.com"));

        _userService.DeleteUser(5, 1);

        _userRepositoryMock.Verify(
            r => r.Delete(It.IsAny<Expression<Func<User, bool>>>()),
            Times.Once);
    }

    [TestMethod]
    public void UpdateUser_SameAsCurrentUser_ThrowsArgumentException()
    {
        var dto = CreateValidUpdateDto();

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser(5, dto, 5));
    }

    [TestMethod]
    public void UpdateUser_UserDoesNotExist_ThrowsKeyNotFoundException()
    {
        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        var dto = CreateValidUpdateDto();

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _userService.UpdateUser(5, dto, 1));
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
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        var dto = CreateValidUpdateDto();

        _userService.UpdateUser(5, dto, 1);

        _userRepositoryMock.Verify(r => r.Update(It.Is<User>(u =>
                u.Id == 5 &&
                u.FirstName == "Juan" &&
                u.LastName == "Garcia" &&
                u.Email == "juan@test.com" &&
                u.Phone == "099123456" &&
                u.Password == HashedPassword)),
            Times.Once);
    }

    [TestMethod]
    public void UpdateUser_ValidData_ReturnsUserExitDto()
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
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        var dto = CreateValidUpdateDto();

        var result = _userService.UpdateUser(5, dto, 1);

        Assert.AreEqual(5, result.Id);
        Assert.AreEqual("Juan", result.FirstName);
        Assert.AreEqual("Admin", result.Role);
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
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        var dto = CreateValidUpdateDto();

        _userService.UpdateUser(5, dto, 1);

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

        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        var dto = CreateValidUpdateDto("taken@test.com");

        Assert.ThrowsException<InvalidOperationException>(() =>
            _userService.UpdateUser(5, dto, 1));
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

        var dto = new RegisterClientEntryDto(
            "Juan",
            "Garcia",
            "juan@test.com",
            "12345",
            "ValidPass@1Ab!xyz");

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _userService.RegisterClient(dto));

        Assert.AreEqual(
            "Phone must be a valid Uruguayan mobile number (09XXXXXXX).",
            ex.Message);
    }

    [TestMethod]
    public void CreateUser_InvalidPhone_ThrowsArgumentException()
    {
        _phoneValidatorMock.Setup(v => v.IsValid("12345")).Returns(false);
        _phoneValidatorMock.Setup(v => v.ErrorMessage)
            .Returns("Phone must be a valid Uruguayan mobile number (09XXXXXXX).");

        var dto = new CreateUserEntryDto(
            "Juan", "Garcia", "juan@test.com", "12345", "ValidPass@1Ab!xyz", "Admin");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.CreateUser(dto));
    }

    [TestMethod]
    public void UpdateUser_InvalidPhone_ThrowsArgumentException()
    {
        var existingUser = CreateUserEntity(5, "Viejo", "Nombre", "viejo@test.com");

        _userRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(existingUser);

        _phoneValidatorMock.Setup(v => v.IsValid("12345")).Returns(false);
        _phoneValidatorMock.Setup(v => v.ErrorMessage)
            .Returns("Phone must be a valid Uruguayan mobile number (09XXXXXXX).");

        var dto = new UpdateUserEntryDto(
            "Juan", "Garcia", "viejo@test.com", "12345", "ValidPass@1Ab!xyz");

        Assert.ThrowsException<ArgumentException>(() =>
            _userService.UpdateUser(5, dto, 1));
    }
}

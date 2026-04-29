using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Test.EntitiesTests;

[TestClass]
public class UserTests
{
    private const string ValidFirstName = "Juan";
    private const string ValidLastName = "Perez";
    private const string ValidEmail = "juan@mail.com";
    private const string ValidPhone = "099123456";
    private const string ValidPassword = "Passw0rd!abcdef";

    [TestMethod]
    public void CreateClient_ValidData_ReturnsUser()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.AreEqual(ValidFirstName, user.FirstName);
        Assert.AreEqual(ValidLastName, user.LastName);
        Assert.AreEqual(ValidEmail, user.Email);
        Assert.AreEqual(ValidPhone, user.Phone);
        Assert.AreEqual(ValidPassword, user.Password);
        Assert.AreEqual(UserRole.Client, user.Role);
    }

    [TestMethod]
    public void CreateClient_EmptyFirstName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(string.Empty, ValidLastName, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_NullFirstName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(null!, ValidLastName, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_WhitespaceFirstName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient("   ", ValidLastName, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_SingleCharFirstName_DoesNotThrow()
    {
        var user = User.CreateClient("A", ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.AreEqual("A", user.FirstName);
    }

    [TestMethod]
    public void CreateClient_EmptyLastName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, string.Empty, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_WhitespaceLastName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, "   ", ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_LastNameTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, "AB", ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_LastNameTooLong_ThrowsArgumentException()
    {
        var longLastName = new string('A', 26);

        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, longLastName, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_LastNameAtMinLength_DoesNotThrow()
    {
        var user = User.CreateClient(ValidFirstName, "ABC", ValidEmail, ValidPhone, ValidPassword);

        Assert.AreEqual("ABC", user.LastName);
    }

    [TestMethod]
    public void CreateClient_LastNameAtMaxLength_DoesNotThrow()
    {
        var maxLastName = new string('A', 25);
        var user = User.CreateClient(ValidFirstName, maxLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.AreEqual(maxLastName, user.LastName);
    }

    [TestMethod]
    public void CreateClient_EmptyEmail_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, string.Empty, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_WhitespaceEmail_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, "   ", ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_EmailWithoutAt_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, "juanmail.com", ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_EmailWithoutDot_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, "juan@mailcom", ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_ValidEmail_DoesNotThrow()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, "test@example.com", ValidPhone, ValidPassword);

        Assert.AreEqual("test@example.com", user.Email);
    }

    [TestMethod]
    public void CreateClient_EmptyPassword_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, string.Empty));
    }

    [TestMethod]
    public void CreateClient_WhitespacePassword_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "   "));
    }

    [TestMethod]
    public void CreateClient_PasswordTooShort_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Short1!abcdefg"));
    }

    [TestMethod]
    public void CreateClient_PasswordTooLong_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Passw0rd!abcdefghijklmnopq"));
    }

    [TestMethod]
    public void CreateClient_PasswordAtMinLength_DoesNotThrow()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Passw0rd!abcdef");

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void CreateClient_PasswordAtMaxLength_DoesNotThrow()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone,
            "Passw0rd!abcdefghijklmnop");

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void CreateClient_PasswordWithoutUppercase_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "passw0rd!abcdef"));
    }

    [TestMethod]
    public void CreateClient_PasswordWithoutLowercase_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "PASSW0RD!ABCDEF"));
    }

    [TestMethod]
    public void CreateClient_PasswordWithoutSymbol_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Passw0rdabcdefg"));
    }

    [TestMethod]
    public void CreateClient_PasswordWithoutDigit_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Password!abcdef"));
    }

    [TestMethod]
    public void CreateClient_PasswordWithNumericSequence_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Pass123!abcdefgh"));
    }

    [TestMethod]
    public void CreateClient_PasswordWithDescendingNumericSequence_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Pass321!abcdefgh"));
    }

    [TestMethod]
    public void CreateClient_PasswordWithNonConsecutiveDigits_DoesNotThrow()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "Pass1a3b5!cdefgh");

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void CreateInternal_AdminRole_ReturnsAdminUser()
    {
        var user = User.CreateInternal(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword, "Admin");

        Assert.AreEqual(UserRole.Admin, user.Role);
    }

    [TestMethod]
    public void CreateInternal_DispatcherRole_ReturnsDispatcherUser()
    {
        var user = User.CreateInternal(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword,
            "Dispatcher");

        Assert.AreEqual(UserRole.Dispatcher, user.Role);
    }

    [TestMethod]
    public void CreateInternal_InvalidRole_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateInternal(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword, "SuperAdmin"));
    }

    [TestMethod]
    public void Update_ValidData_UpdatesAllFields()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        user.Update("Maria", "Gonzalez", "maria@mail.com", "099999999", "NewPassw0rd!abcde");

        Assert.AreEqual("Maria", user.FirstName);
        Assert.AreEqual("Gonzalez", user.LastName);
        Assert.AreEqual("maria@mail.com", user.Email);
        Assert.AreEqual("099999999", user.Phone);
        Assert.AreEqual("NewPassw0rd!abcde", user.Password);
    }

    [TestMethod]
    public void Update_InvalidFirstName_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update(string.Empty, ValidLastName, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void Update_WhitespaceFirstName_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update("   ", ValidLastName, ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void Update_WhitespaceLastName_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update(ValidFirstName, "   ", ValidEmail, ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void Update_WhitespaceEmail_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update(ValidFirstName, ValidLastName, "   ", ValidPhone, ValidPassword));
    }

    [TestMethod]
    public void Update_WhitespacePassword_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, "   "));
    }

    [TestMethod]
    public void CreateClient_EmptyPhone_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, string.Empty, ValidPassword));
    }

    [TestMethod]
    public void CreateClient_WhitespacePhone_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, "   ", ValidPassword));
    }

    [TestMethod]
    public void CreateClient_NullPhone_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, null!, ValidPassword));
    }

    [TestMethod]
    public void Update_EmptyPhone_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update(ValidFirstName, ValidLastName, ValidEmail, string.Empty, ValidPassword));
    }

    [TestMethod]
    public void Update_WhitespacePhone_ThrowsArgumentException()
    {
        var user = User.CreateClient(ValidFirstName, ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.ThrowsException<ArgumentException>(() =>
            user.Update(ValidFirstName, ValidLastName, ValidEmail, "   ", ValidPassword));
    }
}

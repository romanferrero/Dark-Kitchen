namespace DarkKitchen.Domain.Test;

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
    public void CreateClient_SingleCharFirstName_DoesNotThrow()
    {
        var user = User.CreateClient("A", ValidLastName, ValidEmail, ValidPhone, ValidPassword);

        Assert.AreEqual("A", user.FirstName);
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
}

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
}

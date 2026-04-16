namespace DarkKitchen.Domain.Test;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void CreateClient_ValidData_ReturnsUser()
    {
        var user = User.CreateClient("Juan", "Perez", "juan@gmail.com", "099123456", "Passw0rd!abcdef");

        Assert.AreEqual("Juan", user.FirstName);
        Assert.AreEqual("Perez", user.LastName);
        Assert.AreEqual("juan@gmail.com", user.Email);
        Assert.AreEqual("099123456", user.Phone);
        Assert.AreEqual("Passw0rd!abcdef", user.Password);
        Assert.AreEqual(UserRole.Client, user.Role);
    }
}

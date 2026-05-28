using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void Create_ValidData_Succeeds()
    {
        var address = Address.Create("18 de Julio", "1234", "5B");

        Assert.AreEqual("18 de Julio", address.Street);
        Assert.AreEqual("1234", address.DoorNumber);
        Assert.AreEqual("5B", address.Apartment);
    }

    [TestMethod]
    public void Create_EmptyStreet_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Address.Create(string.Empty, "1234", "5B"));
    }

    [TestMethod]
    public void Create_NullStreet_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Address.Create(null!, "1234", "5B"));
    }

    [TestMethod]
    public void Create_EmptyDoorNumber_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Address.Create("18 de Julio", string.Empty, "5B"));
    }

    [TestMethod]
    public void Create_NullDoorNumber_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Address.Create("18 de Julio", null!, "5B"));
    }

    [TestMethod]
    public void Create_NullApartment_IsAllowed()
    {
        var address = Address.Create("18 de Julio", "1234", null!);

        Assert.IsNull(address.Apartment);
    }
}

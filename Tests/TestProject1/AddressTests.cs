namespace DarkKitchen.Domain.Test;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void CreateValid_Address()
    {
        var street = "Av. Siempre Viva";
        var doorNumber = "742";
        var apartment = "A";

        Address address = Address.Create(street, doorNumber, apartment);

        Assert.IsNotNull(address);
        Assert.AreEqual(street, address.Street);
        Assert.AreEqual(doorNumber, address.DoorNumber);
        Assert.AreEqual(apartment, address.Apartment);
    }
}

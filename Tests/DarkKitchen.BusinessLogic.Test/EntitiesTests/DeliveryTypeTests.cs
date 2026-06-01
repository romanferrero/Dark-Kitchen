using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class DeliveryTypeTests
{
    [TestMethod]
    public void Create_ValidData_ReturnsDeliveryType()
    {
        var deliveryType = DeliveryType.Create("Express", 250m);

        Assert.AreEqual("Express", deliveryType.Name);
        Assert.AreEqual(250m, deliveryType.ShippingCost);
    }

    [TestMethod]
    public void Create_EmptyName_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => DeliveryType.Create(string.Empty, 250m));
    }

    [TestMethod]
    public void Create_NegativeShippingCost_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => DeliveryType.Create("Express", -1m));
    }

    [TestMethod]
    public void Update_ValidData_ChangesProperties()
    {
        var deliveryType = DeliveryType.Create("Express", 250m);

        deliveryType.Update("NextDay", 180m);

        Assert.AreEqual("NextDay", deliveryType.Name);
        Assert.AreEqual(180m, deliveryType.ShippingCost);
    }

    [TestMethod]
    public void Update_EmptyName_Throws()
    {
        var deliveryType = DeliveryType.Create("Express", 250m);

        Assert.ThrowsException<ArgumentException>(() => deliveryType.Update(string.Empty, 250m));
    }

    [TestMethod]
    public void Update_NegativeShippingCost_Throws()
    {
        var deliveryType = DeliveryType.Create("Express", 250m);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => deliveryType.Update("Express", -1m));
    }
}

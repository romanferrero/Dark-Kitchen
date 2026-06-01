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
}

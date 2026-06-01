using DarkKitchen.Domain.Deliveries;

namespace DarkKitchen.BusinessLogic.Test.DeliveryTests;

[TestClass]
public class DeliveryHierarchyTests
{
    [TestMethod]
    public void ExpressDelivery_ShippingCost_Returns20()
    {
        var delivery = new ExpressDelivery();

        Assert.AreEqual(20m, delivery.ShippingCost);
    }

    [TestMethod]
    public void TwentyFourHoursDelivery_ShippingCost_Returns10()
    {
        var delivery = new TwentyFourHoursDelivery();

        Assert.AreEqual(10m, delivery.ShippingCost);
    }

    [TestMethod]
    public void FromName_Express_ReturnsExpressDelivery()
    {
        var delivery = Delivery.FromName("Express");

        Assert.IsInstanceOfType(delivery, typeof(ExpressDelivery));
    }

    [TestMethod]
    public void FromName_TwentyFourHours_ReturnsTwentyFourHoursDelivery()
    {
        var delivery = Delivery.FromName("TwentyFourHours");

        Assert.IsInstanceOfType(delivery, typeof(TwentyFourHoursDelivery));
    }

    [TestMethod]
    public void FromName_Unknown_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => Delivery.FromName("INVALID"));
    }
}

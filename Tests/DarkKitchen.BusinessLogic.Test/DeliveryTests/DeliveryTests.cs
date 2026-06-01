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
}

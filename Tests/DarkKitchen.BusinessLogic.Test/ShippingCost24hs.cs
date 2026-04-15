using DarkKitchen.BusinessLogic.ShippingCosts;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ShippingCost24hsCalculatorTests
{
    private ShippingCost24hsCalculator _shippingCost24HsCalculator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _shippingCost24HsCalculator = new ShippingCost24hsCalculator();
    }

    [TestMethod]
    public void Calculate_24hs_Returns10()
    {
        var cost = _shippingCost24HsCalculator.GetCost();

        Assert.AreEqual(10, cost);
    }
}

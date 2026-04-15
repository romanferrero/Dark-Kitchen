using DarkKitchen.BusinessLogic.ShippingCosts;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ShippingCost24hsCalculatorTests
{
    private ShippingCost24hsCalculator _24hsCalculator111 = null!;

    [TestInitialize]
    public void Initialize()
    {
        _24hsCalculator111 = new ShippingCost24hsCalculator();
    }

    [TestMethod]
    public void Calculate_24hs_Returns10()
    {
        var cost = _24hsCalculator111.GetCost();

        Assert.AreEqual(10, cost);
    }
}

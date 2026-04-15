using DarkKitchen.BusinessLogic.Services;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ShippingCost24hsCalculatorTests
{
    private ShippingCost24hsCalculator _24hsCalculator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _24hsCalculator = new ShippingCost24hsCalculator();
    }

    [TestMethod]
    public void Calculate_24hs_Returns10()
    {
        var cost = _24hsCalculator.GetCost();

        Assert.AreEqual(10, cost);
    }
}

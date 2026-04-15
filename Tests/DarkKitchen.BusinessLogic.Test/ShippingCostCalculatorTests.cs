using DarkKitchen.BusinessLogic.Services;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ShippingCostCalculatorTests
{
    private ShippingCostExpressCalculator _expressCalculator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _expressCalculator = new ShippingCostExpressCalculator();
    }

    [TestMethod]
    public void Calculate_Express_Returns20()
    {
        var cost = _expressCalculator.GetCost();

        Assert.AreEqual(20, cost);
    }
}

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
    public void Calculate_Express_Returns20()
    {
        var cost = _24hsCalculator.GetCost();

        Assert.AreEqual(20, cost);
    }
}

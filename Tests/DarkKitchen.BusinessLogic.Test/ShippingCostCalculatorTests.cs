using DarkKitchen.BusinessLogic.Services;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ShippingCostCalculatorTests
{
    private ShippingCostCalculator _calculator = null!;

    [TestInitialize]
    public void Initialize()
    {
        _calculator = new ShippingCostCalculator();
    }

    [TestMethod]
    public void Calculate_Express_Returns100()
    {
        var cost = _calculator.Calculate("express");

        Assert.AreEqual(100m, cost);
    }

    [TestMethod]
    public void Calculate_TwentyFourHours_Returns50()
    {
        var cost = _calculator.Calculate("24hs");

        Assert.AreEqual(50m, cost);
    }

    [TestMethod]
    public void Calculate_InvalidType_ThrowsArgumentException()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _calculator.Calculate("drone"));

        Assert.AreEqual("Delivery type 'drone' is not supported.", ex.Message);
    }
}

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

    
}

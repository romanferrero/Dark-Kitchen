using DarkKitchen.BusinessLogic.ShippingCosts;

namespace DarkKitchen.BusinessLogic.Test.ShippingCostTests;

[TestClass]
public class ShippingCostExpressCalculatorTests
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

        Assert.AreEqual(20M, cost);
    }

    [TestMethod]
    public void GetCost_IsGreaterThanZero()
    {
        var cost = _expressCalculator.GetCost();

        Assert.IsTrue(cost > 0);
    }

    [TestMethod]
    public void GetCost_MultipleCalls_ReturnSameValue()
    {
        var cost1 = _expressCalculator.GetCost();
        var cost2 = _expressCalculator.GetCost();

        Assert.AreEqual(cost1, cost2);
    }

    [TestMethod]
    public void GetCost_ReturnsDecimal()
    {
        var cost = _expressCalculator.GetCost();

        Assert.IsInstanceOfType(cost, typeof(decimal));
    }
}

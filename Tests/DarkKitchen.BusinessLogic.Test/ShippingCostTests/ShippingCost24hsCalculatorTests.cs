using DarkKitchen.BusinessLogic.ShippingCosts;

namespace DarkKitchen.BusinessLogic.Test.ShippingCostTests;

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

    [TestMethod]
    public void GetCost_IsGreaterThanZero()
    {
        var cost = _shippingCost24HsCalculator.GetCost();

        Assert.IsTrue(cost > 0);
    }

    [TestMethod]
    public void GetCost_MultipleCalls_ReturnSameValue()
    {
        var cost1 = _shippingCost24HsCalculator.GetCost();
        var cost2 = _shippingCost24HsCalculator.GetCost();

        Assert.AreEqual(cost1, cost2);
    }

    [TestMethod]
    public void GetCost_ReturnsDecimalValue()
    {
        var cost = _shippingCost24HsCalculator.GetCost();

        Assert.IsInstanceOfType(cost, typeof(decimal));
    }

    [TestMethod]
    public void GetCost_EqualsExpectedDecimal()
    {
        var cost = _shippingCost24HsCalculator.GetCost();

        Assert.AreEqual(10m, cost);
    }
}

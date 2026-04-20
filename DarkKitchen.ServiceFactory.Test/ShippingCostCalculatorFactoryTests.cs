using DarkKitchen.BusinessLogic.ShippingCosts;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IShippingCost;

namespace DarkKitchen.ServiceFactory.Test;

[TestClass]
public class ShippingCostCalculatorFactoryTests
{
    private List<IShippingCostCalculator> _calculators = null!;
    private ShippingCostCalculatorFactory _factory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _calculators =
        [
            new ShippingCostExpressCalculator(),
            new ShippingCost24hsCalculator()
        ];

        _factory = new ShippingCostCalculatorFactory(_calculators);
    }

    [TestMethod]
    public void GetCalculator_Express_ReturnsExpressCalculator()
    {
        var result = _factory.GetCalculator(DeliveryType.Express);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ShippingCostExpressCalculator));
    }

    [TestMethod]
    public void GetCalculator_TwentyFourHours_Returns24hsCalculator()
    {
        var result = _factory.GetCalculator(DeliveryType.TwentyFourHours);

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ShippingCost24hsCalculator));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GetCalculator_Express_WhenCalculatorNotRegistered_ThrowsException()
    {
        List<ShippingCost24hsCalculator> calculators = [new ShippingCost24hsCalculator()];

        var factory = new ShippingCostCalculatorFactory(calculators);

        factory.GetCalculator(DeliveryType.Express);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetCalculator_InvalidDeliveryType_ThrowsArgumentException()
    {
        var invalidType = (DeliveryType)999;

        _factory.GetCalculator(invalidType);
    }
}

using DarkKitchen.BusinessLogic.ShippingCosts;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.ServiceFactory;

public class ShippingCostCalculatorFactory : IShippingCostCalculatorFactory
{
    private readonly IEnumerable<IShippingCostCalculator> _calculators;

    public ShippingCostCalculatorFactory(IEnumerable<IShippingCostCalculator> calculators)
    {
        _calculators = calculators;
    }

    public IShippingCostCalculator GetCalculator(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => _calculators
                .First(c => c is ShippingCostExpressCalculator),

            DeliveryType.TwentyFourHours => _calculators
                .First(c => c is ShippingCost24hsCalculator),

            _ => throw new ArgumentException("Invalid delivery type")
        };
    }
}

using DarkKitchen.BusinessLogic.ShippingCosts;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.ServiceFactory;

public class ShippingCostCalculatorFactory(IEnumerable<IShippingCostCalculator> calculators) : IShippingCostCalculatorFactory
{
    public IShippingCostCalculator GetCalculator(DeliveryType deliveryType)
    {
        return deliveryType switch
        {
            DeliveryType.Express => calculators.First(c => c is ShippingCostExpressCalculator),

            DeliveryType.TwentyFourHours => calculators.First(c => c is ShippingCost24hsCalculator),

            _ => throw new ArgumentException("Invalid delivery type")
        };
    }
}

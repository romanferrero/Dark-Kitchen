using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.ShippingCosts;

public class ShippingCost24hsCalculator : IShippingCostCalculator
{
    private readonly double _shippingCost24hs = 10;
    public double GetCost()
    {
        return _shippingCost24hs;
    }
}

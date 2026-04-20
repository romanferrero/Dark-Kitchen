using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.ShippingCosts;

public class ShippingCost24hsCalculator : IShippingCostCalculator
{
    private readonly decimal _shippingCost24Hs = 10m;
    public decimal GetCost()
    {
        return _shippingCost24Hs;
    }
}

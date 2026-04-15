using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.ShippingCosts;

public class ShippingCost24hsCalculator : IShippingCostCalculator
{
    private readonly double _24hsShippingCost = 10;
    public double GetCost()
    {
        return _24hsShippingCost;
    }
}

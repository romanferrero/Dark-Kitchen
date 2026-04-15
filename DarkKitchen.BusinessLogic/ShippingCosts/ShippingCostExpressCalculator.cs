using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.ShippingCosts;

public class ShippingCostExpressCalculator : IShippingCostCalculator
{
    private readonly double _expressCost = 20;
    public double GetCost()
    {
        return _expressCost;
    }
}

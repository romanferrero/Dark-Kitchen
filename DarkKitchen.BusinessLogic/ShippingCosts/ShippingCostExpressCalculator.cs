using DarkKitchen.IBusinessLogic.IShippingCost;

namespace DarkKitchen.BusinessLogic.ShippingCosts;

public class ShippingCostExpressCalculator : IShippingCostCalculator
{
    private readonly decimal _expressCost = 20m;
    public decimal GetCost()
    {
        return _expressCost;
    }
}

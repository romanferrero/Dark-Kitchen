using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Services;

public class ShippingCost24hsCalculator : IShippingCostCalculator
{
    private double _24hsShippingCost = 10;
    public double GetCost()
    {
        return _24hsShippingCost;
    }
}

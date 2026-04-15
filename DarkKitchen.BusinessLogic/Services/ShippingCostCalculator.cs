using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Services;

public class ShippingCostExpressCalculator : IShippingCostCalculator
{
    private double _expressCost = 20;
    public double Calculate(double subtotal)
    {
        return (double)_expressCost;
    }
}

using DarkKitchen.IBusinessLogic;

namespace DarkKitchen.BusinessLogic.Services;

public class ShippingCostCalculator : IShippingCostCalculator
{
    private static readonly Dictionary<string, decimal> ShippingCosts = new()
    {
        { "express", 100m },
        { "24hs", 50m },
    };

    public decimal Calculate(string deliveryType)
    {
        if(!ShippingCosts.TryGetValue(deliveryType, out var cost))
        {
            throw new ArgumentException($"Delivery type '{deliveryType}' is not supported.");
        }

        return cost;
    }
}

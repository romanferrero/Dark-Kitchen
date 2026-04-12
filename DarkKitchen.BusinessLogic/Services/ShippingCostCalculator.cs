using DarkKitchen.BusinessLogic.Interfaces;

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
        if(deliveryType == "express")
        {
            return ShippingCosts["express"];
        }

        if(deliveryType == "24hs")
        {
            return ShippingCosts["24hs"];
        }

        throw new ArgumentException($"Delivery type '{deliveryType}' is not supported.");
    }
}

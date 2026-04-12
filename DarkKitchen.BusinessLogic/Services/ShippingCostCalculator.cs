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
            return 100m;
        }

        return 50m;
    }
}

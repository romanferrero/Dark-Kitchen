namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IShippingCostCalculator
{
    decimal Calculate(string deliveryType);
}

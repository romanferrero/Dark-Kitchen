namespace DarkKitchen.IBusinessLogic;

public interface IShippingCostCalculator
{
    decimal Calculate(string deliveryType);
}

using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IShippingCostCalculator
{
    decimal Calculate(DeliveryType deliveryType);
}

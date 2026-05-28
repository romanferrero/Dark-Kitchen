using DarkKitchen.Domain.Enums;

namespace DarkKitchen.IBusinessLogic.IShippingCost;

public interface IShippingCostCalculatorFactory
{
    IShippingCostCalculator GetCalculator(DeliveryType deliveryType);
}

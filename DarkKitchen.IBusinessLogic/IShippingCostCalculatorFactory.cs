using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IShippingCostCalculatorFactory
{
    IShippingCostCalculator GetCalculator(DeliveryType deliveryType);
}

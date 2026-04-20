using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic.IShippingCost;

namespace DarkKitchen.IBusinessLogic;

public interface IShippingCostCalculatorFactory
{
    IShippingCostCalculator GetCalculator(DeliveryType deliveryType);
}

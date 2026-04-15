using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderFactory
{
    Order CreateOrder(
        int orderId,
        DeliveryType deliveryType,
        Address address,
        List<Product> products,
        int clientId,
        int orderNumber,
        double subtotal,
        double shippingCost,
        double totalCost);
}

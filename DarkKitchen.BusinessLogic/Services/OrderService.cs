using DarkKitchen.BusinessLogic.DTOS;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository,
    IPromotionRepository promotionRepository) : IOrderService
{
    private const decimal IvaRate = 0.22m;
    private const decimal ExpressShippingCost = 100m;
    private const decimal StandardShippingCost = 50m;

    public OrderResultDTO CreateOrder(
        int clientId,
        string deliveryType,
        string street,
        string doorNumber,
        string apartment,
        List<(string ProductCode, int Quantity)> items)
    {
        var subtotal = 0m;

        foreach (var item in items)
        {
            var product = productRepository.GetByCode(item.ProductCode);
            subtotal += product!.Price * item.Quantity;
        }

        var shippingCost = deliveryType == "express" ? ExpressShippingCost : StandardShippingCost;

        var total = (subtotal + shippingCost) * (1 + IvaRate);

        var order = new Order
        {
            ClientId = clientId,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
        };

        orderRepository.Add(order);

        return new OrderResultDTO
        {
            ClientId = clientId,
            OrderNumber = order.Id,
            Subtotal = subtotal,
            ShippingCost = shippingCost,
            Total = total,
        };
    }
}

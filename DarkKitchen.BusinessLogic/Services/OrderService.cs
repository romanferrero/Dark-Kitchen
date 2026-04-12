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
        if(items.Count == 0)
        {
            throw new ArgumentException("Order must have at least one product.");
        }

        var subtotal = 0m;

        foreach(var item in items)
        {
            var product = productRepository.GetByCode(item.ProductCode);

            if(!product!.Active)
            {
                throw new ArgumentException($"Product '{item.ProductCode}' is inactive and cannot be ordered.");
            }

            subtotal += product.Price * item.Quantity;
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

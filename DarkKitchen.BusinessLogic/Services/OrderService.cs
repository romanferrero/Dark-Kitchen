using DarkKitchen.BusinessLogic.DTOS;
using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
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
        var today = DateOnly.FromDateTime(DateTime.Today);
        var orderItems = new List<OrderItem>();

        foreach(var item in items)
        {
            var product = productRepository.GetByCode(item.ProductCode)
                ?? throw new KeyNotFoundException($"Product '{item.ProductCode}' not found.");

            if(!product.Active)
            {
                throw new ArgumentException($"Product '{item.ProductCode}' is inactive and cannot be ordered.");
            }

            var unitPrice = product.Price;

            var promotions = promotionRepository.GetFiltered(today, null, item.ProductCode);
            if(promotions.Count > 0)
            {
                var highestDiscount = promotions.Max(p => p.DiscountPercentage);
                unitPrice -= unitPrice * highestDiscount / 100m;
            }

            subtotal += unitPrice * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
            });
        }

        var shippingCost = deliveryType == "express" ? ExpressShippingCost : StandardShippingCost;
        var total = (subtotal + shippingCost) * (1 + IvaRate);

        var parsedDeliveryType = deliveryType == "express"
            ? DeliveryType.Express
            : DeliveryType.TwentyFourHours;

        var address = new Address
        {
            Street = street,
            DoorNumber = doorNumber,
            Apartment = apartment,
        };

        var order = Order.Create(clientId, parsedDeliveryType, address, orderItems);

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

using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPromotionRepository promotionRepository,
    IUserRepository userRepository,
    IShippingCostCalculator shippingCostCalculator) : IOrderService
{
    private const decimal IvaRate = 0.22m;

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

        ValidateClientExists(clientId);
        var parsedDeliveryType = ParseDeliveryType(deliveryType);

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

            var orderItem = BuildOrderItem(product, item.Quantity, today);
            subtotal += orderItem.UnitPrice * orderItem.Quantity;
            orderItems.Add(orderItem);
        }

        var shippingCost = shippingCostCalculator.Calculate(deliveryType);
        var total = (subtotal + shippingCost) * (1 + IvaRate);

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

    private void ValidateClientExists(int clientId)
    {
        var client = userRepository.GetById(clientId)
            ?? throw new KeyNotFoundException($"Client with id '{clientId}' not found.");

        if(client.Role != UserRole.Client)
        {
            throw new ArgumentException("Only clients can place orders.");
        }
    }

    private OrderItem BuildOrderItem(Product product, int quantity, DateOnly today)
    {
        var originalPrice = product.Price;
        var unitPrice = originalPrice;
        string? promotionName = null;
        int? discountPercentage = null;

        var promotions = promotionRepository.GetFiltered(today, null, product.Code);
        if(promotions.Count > 0)
        {
            var bestPromotion = promotions.OrderByDescending(p => p.DiscountPercentage).First();
            discountPercentage = bestPromotion.DiscountPercentage;
            promotionName = bestPromotion.Name;
            unitPrice -= unitPrice * discountPercentage.Value / 100m;
        }

        return new OrderItem
        {
            ProductId = product.Id,
            Product = product,
            Quantity = quantity,
            OriginalPrice = originalPrice,
            UnitPrice = unitPrice,
            PromotionName = promotionName,
            DiscountPercentage = discountPercentage,
        };
    }

    private static readonly Dictionary<string, DeliveryType> DeliveryTypeMap = new()
    {
        { "express", DeliveryType.Express },
        { "24hs", DeliveryType.TwentyFourHours },
    };

    private static DeliveryType ParseDeliveryType(string deliveryType)
    {
        if(!DeliveryTypeMap.TryGetValue(deliveryType, out var parsed))
        {
            throw new ArgumentException($"Delivery type '{deliveryType}' is not supported.");
        }

        return parsed;
    }
}

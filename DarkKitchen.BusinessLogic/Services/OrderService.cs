using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUserRepository userRepository,
    IShippingCostCalculatorFactory shippingFactory,
    IPromotionRepository promotionRepository,
    IDiscountCalculator discountCalculator) : IOrderService
{
    private const decimal VatRate = 1.22m;

    public OrderResultDTO CreateOrder(
        int clientId,
        string deliveryType,
        string street,
        string doorNumber,
        string apartment,
        List<string> items)
    {
        var userExists = userRepository.GetAll(u => u.Id == clientId).Any();
        if(!userExists)
        {
            throw new ArgumentException("User not found");
        }

        var products = productRepository.GetAll(p => items.Contains(p.Code)).ToList();

        var inactiveProduct = products.FirstOrDefault(p => !p.Active);
        if(inactiveProduct != null)
        {
            throw new ArgumentException($"Cannot place order: product '{inactiveProduct.Code}' is inactive.");
        }

        var deliveryTypeEnum = Enum.Parse<DeliveryType>(deliveryType);
        var shippingCost = shippingFactory.GetCalculator(deliveryTypeEnum).GetCost();
        var address = Address.Create(street, doorNumber, apartment);
        var activePromotions = GetActivePromotions();

        var subtotal = products.Sum(p => discountCalculator.CalculatePrice(p, activePromotions));
        var total = (subtotal + shippingCost) * VatRate;

        var order = Order.Create(0, deliveryTypeEnum, address, products, clientId, 0, subtotal, shippingCost, total);
        orderRepository.Add(order);

        return new OrderResultDTO
        {
            ClientId = order.ClientId,
            OrderNumber = order.OrderNumber,
            Subtotal = order.Subtotal,
            ShippingCost = order.ShippingCost,
            Total = order.TotalCost
        };
    }

    public UpdateStatusExitDTO UpdateStatus(int orderId, UpdateStatusEntryDTO dto)
    {
        var order = orderRepository.GetAll(o => o.OrderId == orderId).FirstOrDefault()
            ?? throw new KeyNotFoundException("Order not found");

        order.UpdateStatus(Enum.Parse<OrderStatus>(dto.Action));
        orderRepository.Update(order);

        return new UpdateStatusExitDTO(order.OrderStatus.ToString(), DateTime.Now);
    }

    public List<OrderSummaryDTO> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status)
    {
        var statusEnum = status != null ? Enum.Parse<OrderStatus>(status, ignoreCase: true) : (OrderStatus?)null;
        var orders = orderRepository.GetClientOrders(clientId, from, to, statusEnum);

        var fullName = ResolveFullName(clientId);
        return orders.Select(o => ToOrderSummary(o, fullName)).ToList();
    }

    public List<OrderSummaryDTO> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status)
    {
        var statusEnum = status != null ? Enum.Parse<OrderStatus>(status, ignoreCase: true) : (OrderStatus?)null;
        var orders = orderRepository.GetOrdersByDateRange(from, to, street, statusEnum);

        var clientIds = orders.Select(o => o.ClientId).Distinct().ToList();
        var usersByClientId = userRepository
            .GetAll(u => clientIds.Contains(u.Id))
            .ToDictionary(u => u.Id, u => u.FullName);

        return orders.Select(o =>
        {
            var name = usersByClientId.GetValueOrDefault(o.ClientId, "Unknown client");
            return ToOrderSummary(o, name);
        }).ToList();
    }

    public OrderDetailDTO GetOrderById(int orderId)
    {
        var order = orderRepository.GetOrderById(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        var fullName = ResolveFullName(order.ClientId);
        var activePromotions = GetActivePromotions();

        var productDetails = order.Products
            .Select(p => ToOrderProductDetail(p, activePromotions))
            .ToList();

        return new OrderDetailDTO
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = fullName,
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalCost = order.TotalCost,
            Products = productDetails
        };
    }

    private List<Promotion> GetActivePromotions()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return promotionRepository.GetAll(p => p.DateFrom <= today && p.DateTo >= today).ToList();
    }

    private string ResolveFullName(int clientId)
    {
        var user = userRepository.GetAll(u => u.Id == clientId).FirstOrDefault();
        return user?.FullName ?? "Unknown client";
    }

    private static Promotion? FindBestPromotion(Product product, List<Promotion> activePromotions)
    {
        return activePromotions
            .Where(promo => promo.Products.Any(prod => prod.Code == product.Code))
            .OrderByDescending(promo => promo.DiscountPercentage)
            .ThenBy(promo => promo.Name)
            .FirstOrDefault();
    }

    private static OrderProductDetailDTO ToOrderProductDetail(Product product, List<Promotion> activePromotions)
    {
        var bestPromotion = FindBestPromotion(product, activePromotions);

        return new OrderProductDetailDTO
        {
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            PromotionName = bestPromotion?.Name,
            DiscountPercentage = bestPromotion?.DiscountPercentage
        };
    }

    private static OrderSummaryDTO ToOrderSummary(Order order, string clientFullName)
    {
        return new OrderSummaryDTO
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = clientFullName,
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalCost = order.TotalCost,
            ProductCount = order.Products.Count
        };
    }
}

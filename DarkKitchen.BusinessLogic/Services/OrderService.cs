using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUserRepository userRepository,
    IShippingCostCalculatorFactory shippingFactory,
    IPromotionRepository promotionRepository) : IOrderService
{
    public OrderResultDTO CreateOrder(
        int clientId,
        string deliveryType,
        string street,
        string doorNumber,
        string apartment,
        List<string> items)
    {
        try
        {
            var user = userRepository.GetAll(user => user.Id == clientId);
            if(user == null || user.Count == 0)
            {
                throw new ArgumentException("User not found");
            }

            var products = productRepository
                .GetAll(p => items.Contains(p.Code))
                .ToList();

            var inactiveProduct = products.FirstOrDefault(p => !p.Active);
            if(inactiveProduct != null)
            {
                throw new ArgumentException($"Cannot place order: product '{inactiveProduct.Code}' is inactive.");
            }

            var deliveryTypeEnum = Enum.Parse<DeliveryType>(deliveryType);

            var calculator = shippingFactory.GetCalculator(deliveryTypeEnum);
            var shippingCost = calculator.GetCost();

            var address = Address.Create(street, doorNumber, apartment);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var activePromotions = promotionRepository
                .GetAll(p => p.DateFrom <= today && p.DateTo >= today)
                .ToList();

            var subtotal = products.Sum(p => (double)BestDiscountedPrice(p, activePromotions));

            const double vatRate = 1.22;
            var total = (subtotal + shippingCost) * vatRate;

            var order = Order.Create(
                0,
                deliveryTypeEnum,
                address,
                products,
                clientId,
                0,
                subtotal,
                shippingCost,
                total);

            orderRepository.Add(order);

            return new OrderResultDTO
            {
                ClientId = order.ClientId,
                OrderNumber = order.OrderNumber,
                Subtotal = (decimal)order.Subtotal,
                ShippingCost = (decimal)order.ShippingCost,
                Total = (decimal)order.TotalCost
            };
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public List<OrderSummaryDTO> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status)
    {
        var statusEnum = status != null ? Enum.Parse<OrderStatus>(status, ignoreCase: true) : (OrderStatus?)null;

        var orders = orderRepository.GetClientOrders(clientId, from, to, statusEnum);

        return orders.Select(o => ToOrderSummary(o, clientId)).ToList();
    }

    public List<OrderSummaryDTO> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status)
    {
        var statusEnum = status != null ? Enum.Parse<OrderStatus>(status, ignoreCase: true) : (OrderStatus?)null;

        var orders = orderRepository.GetOrdersByDateRange(from, to, street, statusEnum);

        return orders.Select(o => ToOrderSummary(o, o.ClientId)).ToList();
    }

    public OrderDetailDTO GetOrderById(int orderId)
    {
        var order = orderRepository.GetOrderById(orderId)
            ?? throw new KeyNotFoundException($"Order {orderId} not found.");

        var users = userRepository.GetAll(u => u.Id == order.ClientId);
        var user = users.FirstOrDefault();
        var fullName = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;

        var today = DateOnly.FromDateTime(DateTime.Today);
        var activePromotions = promotionRepository
            .GetAll(p => p.DateFrom <= today && p.DateTo >= today)
            .ToList();

        var productDetails = order.Products.Select(p =>
        {
            var bestPromotion = activePromotions
                .Where(promo => promo.Products.Any(prod => prod.Code == p.Code))
                .OrderByDescending(promo => promo.DiscountPercentage)
                .FirstOrDefault();

            return new OrderProductDetailDTO
            {
                Code = p.Code,
                Name = p.Name,
                Price = p.Price,
                Category = p.Category,
                PromotionName = bestPromotion?.Name,
                DiscountPercentage = bestPromotion?.DiscountPercentage
            };
        }).ToList();

        return new OrderDetailDTO
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = fullName,
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalCost = (decimal)order.TotalCost,
            Products = productDetails
        };
    }

    private OrderSummaryDTO ToOrderSummary(Order order, int clientId)
    {
        var users = userRepository.GetAll(u => u.Id == clientId);
        var user = users.FirstOrDefault();
        var fullName = user != null ? $"{user.FirstName} {user.LastName}" : string.Empty;

        return new OrderSummaryDTO
        {
            OrderNumber = order.OrderNumber,
            ClientId = order.ClientId,
            ClientFullName = fullName,
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalCost = (decimal)order.TotalCost,
            ProductCount = order.Products.Count
        };
    }

    private static decimal BestDiscountedPrice(Product product, List<Promotion> activePromotions)
    {
        var bestDiscount = activePromotions
            .Where(p => p.Products.Any(prod => prod.Code == product.Code))
            .Select(p => p.DiscountPercentage)
            .DefaultIfEmpty(0)
            .Max();

        return product.Price * (1 - (bestDiscount / 100m));
    }
}

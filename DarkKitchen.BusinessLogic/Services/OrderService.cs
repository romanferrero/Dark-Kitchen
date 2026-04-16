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
            if(user == null)
            {
                throw new ArgumentException("User not found");
            }

            var products = productRepository
                .GetAll(p => items.Contains(p.Code))
                .ToList();

            var inactiveProduct = products.FirstOrDefault(p => !p.Active);
            if (inactiveProduct != null)
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
        throw new NotImplementedException();
    }

    public List<OrderSummaryDTO> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status)
    {
        throw new NotImplementedException();
    }

    public OrderDetailDTO GetOrderById(int orderId)
    {
        throw new NotImplementedException();
    }

    private static decimal BestDiscountedPrice(Product product, List<Promotion> activePromotions)
    {
        var bestDiscount = activePromotions
            .Where(p => p.Products.Any(prod => prod.Code == product.Code))
            .Select(p => p.DiscountPercentage)
            .DefaultIfEmpty(0)
            .Max();

        return product.Price * (1 - bestDiscount / 100m);
    }
}

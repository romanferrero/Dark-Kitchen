using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IRepository<User> userRepository,
    IShippingCostCalculator shippingCostCalculator,
    IOrderFactory orderFactory) : IOrderService
{
    public OrderResultDTO CreateOrder(
        int clientId,
        string deliveryType,
        string street,
        string doorNumber,
        string apartment,
        List<string> items)
    {
        ValidateClientExists(clientId);

        var products = items
            .Select(productRepository.GetByCode)
            .ToList();

        var deliveryTypeEnum = Enum.Parse<DeliveryType>(deliveryType);
        var address = Address.Create(street, doorNumber, apartment);

        var subtotal = products.Sum(p => (double)p.Price);

        var shippingCost = shippingCostCalculator.GetCost();
        var total = subtotal + shippingCost;

        var order = orderFactory.CreateOrder(
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
            Subtotal = (decimal)subtotal,
            ShippingCost = (decimal)shippingCost,
            Total = (decimal)total
        };
    }

    private void ValidateClientExists(int clientId)
    {
        var client = userRepository.GetAll(u => u.Id == clientId).FirstOrDefault()
                     ?? throw new KeyNotFoundException($"Client with id '{clientId}' not found.");

        if(client.Role != UserRole.Client)
        {
            throw new ArgumentException("Only clients can place orders.");
        }
    }
}

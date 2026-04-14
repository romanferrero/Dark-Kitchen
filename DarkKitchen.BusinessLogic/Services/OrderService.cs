using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IPromotionRepository promotionRepository,
    IUserRepository userRepository,
    IShippingCostCalculator shippingCostCalculator,
    IOrderFactory orderFactory) : IOrderService
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
        return OrderResultDTO();
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
}

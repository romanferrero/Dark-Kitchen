using DarkKitchen.BusinessLogic.DTOS;
using DarkKitchen.BusinessLogic.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository,
    IPromotionRepository promotionRepository) : IOrderService
{
    public OrderResultDTO CreateOrder(int clientId, string deliveryType, string street,
        string doorNumber, string apartment, List<(string ProductCode, int Quantity)> items)
    {
        throw new NotImplementedException();
    }
}

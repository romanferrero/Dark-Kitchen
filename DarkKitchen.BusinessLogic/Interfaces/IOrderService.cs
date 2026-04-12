using DarkKitchen.BusinessLogic.DTOS;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IOrderService
{
    OrderResultDTO CreateOrder(int clientId, string deliveryType, string street, string doorNumber,
        string apartment, List<(string ProductCode, int Quantity)> items);
}

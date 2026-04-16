using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderService
{
    OrderResultDTO CreateOrder(int clientId, string deliveryType, string street, string doorNumber,
        string apartment, List<string> items);

    void UpdateStatus(int orderId, string oneStatus);
}

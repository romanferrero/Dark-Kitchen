using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IOrderService
{
    OrderResultDTO CreateOrder(int clientId, string deliveryType, string street, string doorNumber,
        string apartment, List<string> items);

    UpdateStatusExitDTO UpdateStatus(int orderId, UpdateStatusEntryDTO dto);

    List<OrderSummaryDTO> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status);

    List<OrderSummaryDTO> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status);

    OrderDetailDTO GetOrderById(int orderId);
}

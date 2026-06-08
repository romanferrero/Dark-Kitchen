using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IOrderService
{
    CreateOrderResultExitDto CreateOrder(CreateOrderEntryDto dto);

    UpdateStatusExitDto UpdateStatus(int orderId, UpdateOrderStatusEntryDto dto);

    List<OrderSummaryExitDto> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status);

    List<OrderSummaryExitDto> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status);

    OrderDetailExitDto GetOrderById(int orderId);
}

using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IOrderService
{
    CreateOrderResultExitDto CreateOrder(CreateOrderEntryDto dto);

    UpdateStatusExitDTO UpdateStatus(int orderId, UpdateOrderStatusEntryDTO dto);

    List<OrderSummaryExitDTO> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status);

    List<OrderSummaryExitDTO> GetDispatcherOrders(DateTime from, DateTime to, string? street, string? status);

    OrderDetailExitDTO GetOrderById(int orderId);
}

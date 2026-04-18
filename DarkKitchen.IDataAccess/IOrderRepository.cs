using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository : IRepository<Order>
{
    List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, OrderStatus? status);

    List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, OrderStatus? status);

    Order? GetOrderById(int orderId);
}

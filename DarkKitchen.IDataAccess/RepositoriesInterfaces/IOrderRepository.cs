using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IOrderRepository : IRepository<Order>
{
    List<Order> GetOrdersWithProducts(DateTime dateFrom, DateTime dateTo);

    List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, OrderStatus? status);

    List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, OrderStatus? status);

    Order? GetOrderById(int orderId);
}

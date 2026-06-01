using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IOrderRepository : IRepository<Order>
{
    List<Order> GetOrdersWithProducts(DateTime dateFrom, DateTime dateTo);

    List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status);

    List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, string? status);

    Order? GetOrderById(int orderId);
}

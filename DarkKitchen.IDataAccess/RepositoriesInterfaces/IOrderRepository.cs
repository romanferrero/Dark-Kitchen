using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IOrderRepository : IRepository<Order>
{
    List<TopProductDto> GetTopSellingProducts(DateTime dateFrom, DateTime dateTo, int top);

    List<MonthlySalesDto> GetMonthlySalesGroupedByClient(List<User> users);
    List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, OrderStatus? status);

    List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, OrderStatus? status);

    Order? GetOrderById(int orderId);
}

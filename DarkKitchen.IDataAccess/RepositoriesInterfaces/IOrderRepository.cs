using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IOrderRepository : IRepository<Order>
{
    List<TopProductExitDTO> GetTopSellingProducts(DateTime dateFrom, DateTime dateTo, int top);

    List<MonthlySalesExitDTO> GetMonthlySalesGroupedByClient(List<User> users);

    List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, OrderStatus? status);

    List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, OrderStatus? status);

    Order? GetOrderById(int orderId);
}

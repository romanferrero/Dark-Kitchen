using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository : IRepository<Order>
{
    List<TopProductDto> GetTopSellingProducts(DateTime dateFrom, DateTime dateTo, int top);

    List<MonthlySalesDto> GetMonthlySalesGroupedByClient(List<User> users);
}

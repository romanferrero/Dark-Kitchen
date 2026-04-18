using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public List<TopProductDto> GetTopSellingProducts(DateTime dateFrom, DateTime dateTo, int top)
    {
        return new List<TopProductDto>();
    }

    public List<MonthlySalesDto> GetMonthlySalesGroupedByClient(List<User> users)
    {
        throw new NotImplementedException();
    }
}

using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, OrderStatus? status)
    {
        throw new NotImplementedException();
    }

    public List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, OrderStatus? status)
    {
        throw new NotImplementedException();
    }

    public Order? GetOrderById(int orderId)
    {
        throw new NotImplementedException();
    }
}

using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public void Add(Order order)
    {
        context.Orders.Add(order);
        context.SaveChanges();
    }
}

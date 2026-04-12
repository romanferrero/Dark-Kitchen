using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public void Add(Order order)
    {
        throw new NotImplementedException();
    }
}

using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
}

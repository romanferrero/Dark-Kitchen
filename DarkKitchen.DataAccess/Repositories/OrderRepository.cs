using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, OrderStatus? status)
    {
        var query = Context.Orders
            .Include(o => o.Products)
            .Where(o => o.ClientId == clientId);

        if(from.HasValue)
        {
            query = query.Where(o => o.OrderDate >= from.Value);
        }

        if(to.HasValue)
        {
            query = query.Where(o => o.OrderDate <= to.Value);
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }

        return query
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, OrderStatus? status)
    {
        var query = Context.Orders
            .Include(o => o.Products)
            .Where(o => o.OrderDate >= from && o.OrderDate <= to);

        if(!string.IsNullOrWhiteSpace(street))
        {
            var streetFilter = street.Trim();
            query = query.Where(o => o.Address.Street.Contains(streetFilter));
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }

        return query
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public Order? GetOrderById(int orderId)
    {
        var order = Context.Orders
            .Include(o => o.Products)
            .FirstOrDefault(o => o.OrderId == orderId);

        if(order != null)
        {
            order.Products = order.Products.OrderBy(p => p.Code).ToList();
        }

        return order;
    }
}

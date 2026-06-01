using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    private readonly AppDbContext _context = context;

    public List<Order> GetOrdersWithProducts(DateTime dateFrom, DateTime dateTo)
    {
        return _context.Orders
            .Include(o => o.Products)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Images)
            .Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo)
            .ToList();
    }

    public List<Order> GetClientOrders(int clientId, DateTime? from, DateTime? to, string? status)
    {
        var query = _context.Orders
            .Include(o => o.Products)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Images)
            .Where(o => o.ClientId == clientId);

        if(from.HasValue)
        {
            query = query.Where(o => o.OrderDate >= from.Value);
        }

        if(to.HasValue)
        {
            query = query.Where(o => o.OrderDate <= to.Value);
        }

        if(status != null)
        {
            query = query.Where(o => o.OrderStatus == status);
        }

        return query
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public List<Order> GetOrdersByDateRange(DateTime from, DateTime to, string? street, string? status)
    {
        var query = _context.Orders
            .Include(o => o.Products)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Images)
            .Where(o => o.OrderDate >= from && o.OrderDate <= to);

        if(!string.IsNullOrWhiteSpace(street))
        {
            var streetFilter = street.Trim();
            query = query.Where(o => o.Address.Street != null && o.Address.Street.Contains(streetFilter));
        }

        if(status != null)
        {
            query = query.Where(o => o.OrderStatus == status);
        }

        return query
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public Order? GetOrderById(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.Products)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Images)
            .FirstOrDefault(o => o.OrderId == orderId);

        if(order != null)
        {
            order.Products = order.Products
                .OrderBy(op => op.Product.Code)
                .ToList();
        }

        return order;
    }
}

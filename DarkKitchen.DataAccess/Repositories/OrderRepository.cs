using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class OrderRepository(AppDbContext context) : Repository<Order>(context), IOrderRepository
{
    public List<TopProductDto> GetTopSellingProducts(DateTime dateFrom, DateTime dateTo, int top)
    {
        var orders = Context.Set<Order>()
            .Include(o => o.Products)
            .ThenInclude(p => p.Images)
            .Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo)
            .ToList();

        return orders
            .SelectMany(o => o.Products)
            .GroupBy(p => new { p.Code, p.Name })
            .Select(g => new TopProductDto
            {
                Code = g.Key.Code,
                Name = g.Key.Name,
                QuantitySold = g.Count(),
                ImageUrls = g
                    .SelectMany(p => p.Images)
                    .Select(i => i.Url)
                    .Distinct()
                    .ToList()
            })
            .OrderByDescending(t => t.QuantitySold)
            .Take(top)
            .ToList();
    }

    public List<MonthlySalesDto> GetMonthlySalesGroupedByClient(List<User> users)
    {
        var orders = Context.Set<Order>().ToList();

        return orders
            .GroupBy(o => o.OrderDate.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(monthGroup =>
            {
                var clientSales = monthGroup
                    .GroupBy(o => o.ClientId)
                    .Select(clientGroup =>
                    {
                        var client = users.FirstOrDefault(u => u.Id == clientGroup.Key);
                        var clientName = client != null
                            ? $"{client.FirstName} {client.LastName}"
                            : $"Cliente {clientGroup.Key}";

                        return new ClientSalesDto
                        {
                            ClientName = clientName,
                            Total = (decimal)clientGroup.Sum(o => o.TotalCost)
                        };
                    })
                    .OrderByDescending(c => c.Total)
                    .ToList();

                return new MonthlySalesDto
                {
                    Period = monthGroup.Key,
                    ClientSales = clientSales,
                    MonthlyTotal = clientSales.Sum(c => c.Total)
                };
            })
            .ToList();
    }
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

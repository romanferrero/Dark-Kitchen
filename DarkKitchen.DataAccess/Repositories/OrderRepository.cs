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
                            ClientName = clientName, Total = (decimal)clientGroup.Sum(o => o.TotalCost)
                        };
                    })
                    .OrderByDescending(c => c.Total)
                    .ToList();

                return new MonthlySalesDto
                {
                    Period = monthGroup.Key, ClientSales = clientSales, MonthlyTotal = clientSales.Sum(c => c.Total)
                };
            })
            .ToList();
    }
}

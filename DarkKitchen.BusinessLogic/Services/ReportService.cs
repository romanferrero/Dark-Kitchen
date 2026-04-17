using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class ReportService(
    IRepository<Order> orderRepository,
    IRepository<User> userRepository) : IReportService
{
    public List<TopProductDto> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        var orders = orderRepository.GetAll(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo);

        return orders
            .SelectMany(o => o.Products)
            .GroupBy(p => p.Code)
            .Select(g => new TopProductDto
            {
                Code = g.Key,
                Name = g.First().Name,
                QuantitySold = g.Count(),
                ImageUrls = g.First().Images.Select(i => i.Url).ToList()
            })
            .OrderByDescending(t => t.QuantitySold)
            .Take(5)
            .ToList();
    }

    public SalesReportDto GetSalesReport()
    {
        var allOrders = orderRepository.GetAll();
        var allUsers = userRepository.GetAll();

        var monthlySales = allOrders
            .GroupBy(o => o.OrderDate.ToString("yyyy-MM"))
            .OrderBy(g => g.Key)
            .Select(monthGroup =>
            {
                var clientSales = monthGroup
                    .GroupBy(o => o.ClientId)
                    .Select(clientGroup =>
                    {
                        var client = allUsers.FirstOrDefault(u => u.Id == clientGroup.Key);
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

        return new SalesReportDto { MonthlySales = monthlySales, GrandTotal = monthlySales.Sum(m => m.MonthlyTotal) };
    }
}

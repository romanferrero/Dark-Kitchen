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
        var orders = orderRepository.GetAll()
            .Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo)
            .ToList();

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
        return new SalesReportDto();
    }
}

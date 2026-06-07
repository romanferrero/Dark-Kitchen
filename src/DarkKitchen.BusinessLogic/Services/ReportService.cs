using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class ReportService(
    IOrderRepository orderRepository,
    IRepository<User> userRepository) : IReportService
{
    private const int TopProductsCount = 5;

    public List<TopProductExitDto> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        var orders = orderRepository.GetOrdersWithProducts(dateFrom, dateTo);

        return orders
            .SelectMany(o => o.Products)
            .GroupBy(op => new { op.Product.Code, op.Product.Name })
            .Select(g => new TopProductExitDto
            {
                Code = g.Key.Code,
                Name = g.Key.Name,
                QuantitySold = g.Sum(op => op.Quantity),
                ImageUrls = g
                    .SelectMany(op => op.Product.Images)
                    .Select(i => i.Url)
                    .Distinct()
                    .ToList()
            })
            .OrderByDescending(t => t.QuantitySold)
            .Take(TopProductsCount)
            .ToList();
    }

    public SalesReportExitDto GetSalesReport()
    {
        var allUsers = userRepository.GetAll();
        var orders = orderRepository.GetAll();

        var monthlySales = orders
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
                            : $"Client {clientGroup.Key}";

                        return new ClientSalesExitDto
                        {
                            ClientName = clientName,
                            Total = clientGroup.Sum(o => o.TotalCost)
                        };
                    })
                    .OrderByDescending(c => c.Total)
                    .ToList();

                return new MonthlySalesExitDto
                {
                    Period = monthGroup.Key,
                    ClientSales = clientSales,
                    MonthlyTotal = clientSales.Sum(c => c.Total)
                };
            })
            .ToList();

        return new SalesReportExitDto
        {
            MonthlySales = monthlySales,
            GrandTotal = monthlySales.Sum(m => m.MonthlyTotal)
        };
    }
}

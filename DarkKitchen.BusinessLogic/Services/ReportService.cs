using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class ReportService(
    IOrderRepository orderRepository,
    IRepository<User> userRepository) : IReportService
{
    private const int TopProductsCount = 5;

    public List<TopProductDto> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        return orderRepository.GetTopSellingProducts(dateFrom, dateTo, TopProductsCount);
    }

    public SalesReportDto GetSalesReport()
    {
        var allUsers = userRepository.GetAll();
        var monthlySales = orderRepository.GetMonthlySalesGroupedByClient(allUsers);

        return new SalesReportDto
        {
            MonthlySales = monthlySales,
            GrandTotal = monthlySales.Sum(m => m.MonthlyTotal)
        };
    }
}

using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class ReportService(
    IOrderRepository orderRepository,
    IRepository<User> userRepository) : IReportService
{
    private const int TopProductsCount = 5;

    public List<TopProductExitDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        return orderRepository.GetTopSellingProducts(dateFrom, dateTo, TopProductsCount);
    }

    public SalesReportExitDTO GetSalesReport()
    {
        var allUsers = userRepository.GetAll();
        var monthlySales = orderRepository.GetMonthlySalesGroupedByClient(allUsers);

        return new SalesReportExitDTO
        {
            MonthlySales = monthlySales,
            GrandTotal = monthlySales.Sum(m => m.MonthlyTotal)
        };
    }
}

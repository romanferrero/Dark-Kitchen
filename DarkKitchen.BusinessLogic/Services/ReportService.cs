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
        throw new NotImplementedException();
    }

    public SalesReportDto GetSalesReport()
    {
        throw new NotImplementedException();
    }
}

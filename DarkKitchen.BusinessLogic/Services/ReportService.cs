using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class ReportService(
    IOrderRepository orderRepository) : IReportService
{
    public List<TopProductDto> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        return orderRepository.GetTopSellingProducts(dateFrom, dateTo, 5);
    }
}

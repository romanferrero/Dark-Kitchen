using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class ReportService(
    IOrderRepository orderRepository) : IReportService
{
    private const int TopProductsCount = 5;

    public List<TopProductDto> GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        return orderRepository.GetTopSellingProducts(dateFrom, dateTo, TopProductsCount);
    }
}

using DarkKitchen.Domain.DTOs;

namespace DarkKitchen.IBusinessLogic;

public interface IReportService
{
    List<TopProductDto> GetTopProducts(DateTime dateFrom, DateTime dateTo);

    SalesReportDto GetSalesReport();
}

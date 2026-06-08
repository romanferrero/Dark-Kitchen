using DarkKitchen.IBusinessLogic.DTOs.Exit;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IReportService
{
    List<TopProductExitDto> GetTopProducts(DateTime dateFrom, DateTime dateTo);

    SalesReportExitDto GetSalesReport();
}

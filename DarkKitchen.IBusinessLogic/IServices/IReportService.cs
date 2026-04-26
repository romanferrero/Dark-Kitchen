using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IReportService
{
    List<TopProductExitDto> GetTopProducts(DateTime dateFrom, DateTime dateTo);

    SalesReportExitDto GetSalesReport();
}

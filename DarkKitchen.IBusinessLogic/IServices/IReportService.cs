using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IReportService
{
    List<TopProductExitDTO> GetTopProducts(DateTime dateFrom, DateTime dateTo);

    SalesReportExitDTO GetSalesReport();
}

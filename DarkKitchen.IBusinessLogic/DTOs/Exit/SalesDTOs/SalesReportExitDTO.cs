namespace DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;

public class SalesReportExitDTO
{
    public List<MonthlySalesExitDTO> MonthlySales { get; set; } = [];
    public decimal GrandTotal { get; set; }
}

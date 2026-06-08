namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public class SalesReportExitDto
{
    public List<MonthlySalesExitDto> MonthlySales { get; set; } = [];
    public decimal GrandTotal { get; set; }
}

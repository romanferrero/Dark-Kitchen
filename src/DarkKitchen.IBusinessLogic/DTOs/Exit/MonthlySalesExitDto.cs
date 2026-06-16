namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public class MonthlySalesExitDto
{
    public string Period { get; set; } = string.Empty;
    public List<ClientSalesExitDto> ClientSales { get; set; } = [];
    public decimal MonthlyTotal { get; set; }
}

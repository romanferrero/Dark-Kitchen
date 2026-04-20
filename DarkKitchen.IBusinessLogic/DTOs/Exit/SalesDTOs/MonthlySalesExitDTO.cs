namespace DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;

public class MonthlySalesExitDTO
{
    public string Period { get; set; } = string.Empty;
    public List<ClientSalesExitDTO> ClientSales { get; set; } = [];
    public decimal MonthlyTotal { get; set; }
}

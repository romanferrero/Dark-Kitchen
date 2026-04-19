namespace DarkKitchen.Domain.DTOs;

public class MonthlySalesDto
{
    public string Period { get; set; } = string.Empty;
    public List<ClientSalesDto> ClientSales { get; set; } = [];
    public decimal MonthlyTotal { get; set; }
}

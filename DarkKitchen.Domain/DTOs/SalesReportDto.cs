namespace DarkKitchen.Domain.DTOs;

public class SalesReportDto
{
    public List<MonthlySalesDto> MonthlySales { get; set; } = [];
    public decimal GrandTotal { get; set; }
}

namespace DarkKitchen.WebApi.Models;

public class GetPromotionsQueryModel
{
    public string? Date { get; set; }
    public string? Line { get; set; }
    public string? Product { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}

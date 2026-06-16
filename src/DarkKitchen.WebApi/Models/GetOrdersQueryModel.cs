namespace DarkKitchen.WebApi.Models;

public class GetOrdersQueryModel
{
    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public string? Status { get; set; }

    public string? Street { get; set; }

    public string? ProductName { get; set; }

    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }
}

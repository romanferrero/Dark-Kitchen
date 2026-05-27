namespace DarkKitchen.WebApi.Models;

public class GetOrdersQueryModel
{
    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public string? Status { get; set; }

    public string? Street { get; set; }
}

namespace DarkKitchen.Domain;

public class Promotion
{
    private Promotion()
    {
    }

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DiscountPercentage { get; set; }

    public DateOnly DateFrom { get; set; }

    public DateOnly DateTo { get; set; }

    public static Promotion Create(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        return new Promotion
        {
            Name = name,
            DiscountPercentage = discountPercentage,
            DateFrom = dateFrom,
            DateTo = dateTo,
        };
    }
}

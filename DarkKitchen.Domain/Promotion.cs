namespace DarkKitchen.Domain;

public class Promotion
{
    private Promotion()
    {
    }

    public int Id { get; set; }

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Promotion name cannot be empty.");
            }

            _name = value;
        }
    }

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

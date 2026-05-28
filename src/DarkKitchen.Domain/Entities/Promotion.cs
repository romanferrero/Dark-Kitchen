namespace DarkKitchen.Domain.Entities;

public class Promotion
{
    private const decimal MinDiscount = 1;
    private const decimal MaxDiscount = 100;

    public int Id { get; set; }
    private string _name = string.Empty;
    private decimal _discountPercentage;
    public DateOnly DateFrom { get; set; }

    public DateOnly DateTo { get; set; }

    public List<Product> Products { get; } = [];

    private Promotion()
    {
    }

    public static Promotion Create(string name, decimal discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        return new Promotion
        {
            Name = name,
            DiscountPercentage = discountPercentage,
            DateFrom = dateFrom,
            DateTo = dateTo,
        };
    }

    public void AddProduct(Product product)
    {
        var alreadyAssociated = Products.Any(p => p.Code == product.Code);
        if(alreadyAssociated)
        {
            throw new InvalidOperationException($"Product '{product.Code}' is already associated to this promotion.");
        }

        Products.Add(product);
    }

    public void RemoveProduct(string productCode)
    {
        var product = Products.FirstOrDefault(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException(
                          $"Product '{productCode}' is not associated to this promotion.");

        Products.Remove(product);
    }

    public void Update(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        ValidateDateRange(dateFrom, dateTo);

        Name = name;
        DiscountPercentage = discountPercentage;
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public string Name
    {
        get => _name;
        set
        {
            ValidateName(value);
            _name = value;
        }
    }

    public decimal DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            ValidateDiscount(value);
            _discountPercentage = value;
        }
    }

    private static void ValidateDateRange(DateOnly dateFrom, DateOnly dateTo)
    {
        if(dateTo < dateFrom)
        {
            throw new ArgumentException("DateTo must be greater than or equal to DateFrom.");
        }
    }

    private static void ValidateName(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Promotion name cannot be empty.");
        }
    }

    private static void ValidateDiscount(decimal value)
    {
        var isOutOfRange = value < MinDiscount || value > MaxDiscount;
        if(isOutOfRange)
        {
            throw new ArgumentException($"Discount must be between {MinDiscount} and {MaxDiscount}.");
        }
    }
}

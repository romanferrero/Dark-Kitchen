namespace DarkKitchen.Domain.Entities;

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

    private decimal _discountPercentage;

    public decimal DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            if(value < 1 || value > 100)
            {
                throw new ArgumentException("Discount must be between 1 and 100.");
            }

            _discountPercentage = value;
        }
    }

    public DateOnly DateFrom { get; set; }

    public DateOnly DateTo { get; set; }

    public List<Product> Products { get; private set; } = [];

    public void Update(string name, int discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        if(dateTo < dateFrom)
        {
            throw new ArgumentException("DateTo must be greater than or equal to DateFrom.");
        }

        Name = name;
        DiscountPercentage = discountPercentage;
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public void AddProduct(Product product)
    {
        if(Products.Any(p => p.Code == product.Code))
        {
            throw new InvalidOperationException($"Product '{product.Code}' is already associated to this promotion.");
        }

        Products.Add(product);
    }

    public void RemoveProduct(string productCode)
    {
        var product = Products.FirstOrDefault(p => p.Code == productCode)
            ?? throw new KeyNotFoundException($"Product '{productCode}' is not associated to this promotion.");

        Products.Remove(product);
    }

    public static Promotion Create(string name, decimal discountPercentage, DateOnly dateFrom, DateOnly dateTo)
    {
        if(dateTo < dateFrom)
        {
            throw new ArgumentException("DateTo must be greater than or equal to DateFrom.");
        }

        return new Promotion
        {
            Name = name,
            DiscountPercentage = discountPercentage,
            DateFrom = dateFrom,
            DateTo = dateTo,
        };
    }
}

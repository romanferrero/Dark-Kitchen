namespace DarkKitchen.Domain.Entities;

public class ProductImage
{
    private const decimal MaxSizeInKb = 500;

    private decimal _sizeInKb;

    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public decimal SizeInKb
    {
        get => _sizeInKb;
        set
        {
            ValidateSize(value);
            _sizeInKb = value;
        }
    }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    private static void ValidateSize(decimal value)
    {
        if(value > MaxSizeInKb)
        {
            throw new ArgumentException($"Product image size cannot exceed {MaxSizeInKb}kb.");
        }
    }
}

namespace DarkKitchen.Domain.Entities;

public class OrderProduct
{
    private const int MinQuantity = 1;

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    private int _quantity;

    public int Quantity
    {
        get => _quantity;
        set
        {
            ValidateQuantity(value);
            _quantity = value;
        }
    }

    private static void ValidateQuantity(int value)
    {
        if(value < MinQuantity)
        {
            throw new ArgumentException($"Quantity must be at least {MinQuantity}.");
        }
    }
}

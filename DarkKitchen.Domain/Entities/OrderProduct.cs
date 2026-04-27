namespace DarkKitchen.Domain.Entities;

public class OrderProduct
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    private int _quantity;

    public int Quantity
    {
        get => _quantity;
        set
        {
            if(value <= 0)
            {
                throw new ArgumentException("Quantity must be at least 1.");
            }

            _quantity = value;
        }
    }
}

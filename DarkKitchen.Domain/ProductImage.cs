namespace DarkKitchen.Domain;

public class ProductImage
{
    private decimal _sizeInKb;

    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public decimal SizeInKb
    {
        get => _sizeInKb;
        set
        {
            if(value > 500)
            {
                throw new ArgumentException("Product image size cannot exceed 500kb.");
            }

            _sizeInKb = value;
        }
    }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

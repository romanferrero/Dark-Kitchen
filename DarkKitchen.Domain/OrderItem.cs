namespace DarkKitchen.Domain;

public class OrderItem
{
    private int _id;
    private int _productId;
    private Product _product = null!;
    private int _quantity;
    private decimal _originalPrice;
    private decimal _unitPrice;
    private string? _promotionName;
    private int? _discountPercentage;
    private int _orderId;
    private Order _order = null!;

    private OrderItem() { }
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public int ProductId
    {
        get => _productId;
        private set => _productId = value;
    }

    public Product Product
    {
        get => _product;
        private set => _product = value;
    }

    public int Quantity
    {
        get => _quantity;
        private set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Quantity must be positive.", nameof(value));
            }

            _quantity = value;
        }
    }

    public decimal OriginalPrice
    {
        get => _originalPrice;
        private set
        {
            if(value <= 0)
            {
                throw new ArgumentException("OriginalPrice must be positive.");
            }

            _originalPrice = value;
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        private set
        {
            if(value <= 0)
            {
                throw new ArgumentException("Unit price must be positive.");
            }

            _unitPrice = value;
        }
    }

    public string? PromotionName
    {
        get => _promotionName;
        private set => _promotionName = value;
    }

    public int? DiscountPercentage
    {
        get => _discountPercentage;
        private set => _discountPercentage = value;
    }

    public int OrderId
    {
        get => _orderId;
        set => _orderId = value;
    }

    public Order Order
    {
        get => _order;
        set => _order = value;
    }

    public static OrderItem Create(
        Product product,
        int quantity,
        decimal originalPrice,
        decimal unitPrice,
        string? promotionName,
        int? discountPercentage)
    {
        return new OrderItem
        {
            ProductId = product.Id,
            Product = product,
            Quantity = quantity,
            OriginalPrice = originalPrice,
            UnitPrice = unitPrice,
            PromotionName = promotionName,
            DiscountPercentage = discountPercentage,
        };
    }
}

namespace DarkKitchen.Domain;

public class OrderItem
{
    private OrderItem() { }

    public int Id { get; set; }
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal OriginalPrice { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string? PromotionName { get; private set; }
    public int? DiscountPercentage { get; private set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

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

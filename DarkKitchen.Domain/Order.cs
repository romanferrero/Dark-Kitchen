namespace DarkKitchen.Domain;

public class Order
{
    private Order()
    {
    }

    public int Id { get; set; }

    public int ClientId { get; set; }

    public User Client { get; set; } = null!;

    public OrderStatus Status { get; set; }

    public DeliveryType DeliveryType { get; set; }

    public Address Address { get; set; } = null!;

    public List<OrderItem> Items { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public static Order Create(
        int clientId,
        DeliveryType deliveryType,
        Address address,
        List<OrderItem> items)
    {
        return new Order
        {
            ClientId = clientId,
            DeliveryType = deliveryType,
            Address = address,
            Items = items,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
        };
    }
}

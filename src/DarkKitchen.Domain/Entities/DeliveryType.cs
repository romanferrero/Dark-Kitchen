namespace DarkKitchen.Domain.Entities;

public class DeliveryType
{
    private DeliveryType() { }

    public static DeliveryType Create(string name, decimal shippingCost)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Delivery type name cannot be empty.");

        ArgumentOutOfRangeException.ThrowIfNegative(shippingCost);

        return new DeliveryType
        {
            Name = name,
            ShippingCost = shippingCost
        };
    }

    public void Update(string name, decimal shippingCost)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Delivery type name cannot be empty.");
        }

        ArgumentOutOfRangeException.ThrowIfNegative(shippingCost);

        Name = name;
        ShippingCost = shippingCost;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal ShippingCost { get; private set; }
}

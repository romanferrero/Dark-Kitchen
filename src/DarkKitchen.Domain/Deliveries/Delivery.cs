namespace DarkKitchen.Domain.Deliveries;

public abstract class Delivery
{
    public abstract decimal ShippingCost { get; }
    public abstract string Name { get; }

    public static Delivery FromName(string name) => name switch
    {
        "Express" => new ExpressDelivery(),
        "TwentyFourHours" => new TwentyFourHoursDelivery(),
        _ => throw new ArgumentException($"Unknown delivery type: '{name}'")
    };
}

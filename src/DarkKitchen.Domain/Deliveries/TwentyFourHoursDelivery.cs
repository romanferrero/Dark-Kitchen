namespace DarkKitchen.Domain.Deliveries;

public class TwentyFourHoursDelivery : Delivery
{
    public override decimal ShippingCost => 10m;
    public override string Name => "TwentyFourHours";
}

namespace DarkKitchen.Domain.Deliveries;

public class ExpressDelivery : Delivery
{
    public override decimal ShippingCost => 20m;
}

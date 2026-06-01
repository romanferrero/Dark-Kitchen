namespace DarkKitchen.Domain.States;

public class OnTheWayOrderState : IOrderState
{
    public string Name => "OnTheWay";

    public IOrderState TransitionTo(string targetState) => targetState switch
    {
        "Delivered"    => new DeliveredOrderState(),
        "NotDelivered" => new NotDeliveredOrderState(),
        _              => throw new ArgumentException("Order must be on the way")
    };
}

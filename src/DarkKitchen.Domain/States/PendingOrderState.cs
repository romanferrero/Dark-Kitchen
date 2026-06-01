namespace DarkKitchen.Domain.States;

public class PendingOrderState : IOrderState
{
    public string Name => "Pending";

    public IOrderState TransitionTo(string targetState) => targetState switch
    {
        "Pending"   => new PendingOrderState(),
        "Prepared"  => new PreparedOrderState(),
        "Cancelled" => new CancelledOrderState(),
        "Delayed"   => new DelayedOrderState(),
        _           => throw new ArgumentException($"Only pending orders can transition to '{targetState}'")
    };
}

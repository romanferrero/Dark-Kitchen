namespace DarkKitchen.Domain.States;

public class PendingOrderState : IOrderState
{
    public string Name => "Pending";

    public IOrderState TransitionTo(string targetState) => targetState switch
    {
        "Pending"   => new PendingOrderState(),
        "Prepared"  => new PreparedOrderState(),
        "Cancelled" => new CancelledOrderState(),
        _           => throw new ArgumentException($"Only pending orders can transition to '{targetState}'")
    };
}

namespace DarkKitchen.Domain.States;

public class DelayedOrderState : IOrderState
{
    public string Name => "Delayed";

    public IOrderState TransitionTo(string targetState) => targetState switch
    {
        "Prepared"  => new PreparedOrderState(),
        "Cancelled" => new CancelledOrderState(),
        _           => throw new ArgumentException($"Only delayed orders can transition to '{targetState}'")
    };
}

namespace DarkKitchen.Domain.States;

public class CancelledOrderState : IOrderState
{
    public string Name => "Cancelled";

    public IOrderState TransitionTo(string targetState) =>
        throw new ArgumentException("Only pending orders can be cancelled");
}

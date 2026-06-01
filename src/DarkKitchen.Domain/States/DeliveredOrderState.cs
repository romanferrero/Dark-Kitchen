namespace DarkKitchen.Domain.States;

public class DeliveredOrderState : IOrderState
{
    public string Name => "Delivered";

    public IOrderState TransitionTo(string targetState) =>
        throw new ArgumentException("Order must be on the way");
}

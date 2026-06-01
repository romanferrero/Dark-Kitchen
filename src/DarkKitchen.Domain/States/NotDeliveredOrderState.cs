namespace DarkKitchen.Domain.States;

public class NotDeliveredOrderState : IOrderState
{
    public string Name => "NotDelivered";

    public IOrderState TransitionTo(string targetState) =>
        throw new ArgumentException("Order must be on the way");
}

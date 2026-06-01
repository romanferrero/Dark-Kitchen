namespace DarkKitchen.Domain.States;

public class PreparedOrderState : IOrderState
{
    public string Name => "Prepared";

    public IOrderState TransitionTo(string targetState) => targetState switch
    {
        "OnTheWay" => new OnTheWayOrderState(),
        _          => throw new ArgumentException("Only pending orders can be prepared")
    };
}

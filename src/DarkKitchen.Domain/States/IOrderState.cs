namespace DarkKitchen.Domain.States;

public interface IOrderState
{
    string Name { get; }
    IOrderState TransitionTo(string targetState);
}

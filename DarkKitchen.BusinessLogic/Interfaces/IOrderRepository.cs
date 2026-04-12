using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IOrderRepository
{
    void Add(Order order);
}

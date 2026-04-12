using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IOrderRepository
{
    void Add(Order order);
}

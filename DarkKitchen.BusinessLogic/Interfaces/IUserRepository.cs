using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IUserRepository
{
    void Add(User user);

    User? GetByEmail(string email);
}

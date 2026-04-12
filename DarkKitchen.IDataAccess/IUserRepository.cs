using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IUserRepository
{
    void Add(User user);

    User? GetByEmail(string email);
}

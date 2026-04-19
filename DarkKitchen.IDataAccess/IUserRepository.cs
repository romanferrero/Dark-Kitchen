using DarkKitchen.Domain;

namespace DarkKitchen.IDataAccess;

public interface IUserRepository : IRepository<User>
{
    User? GetByEmail(string email);
}

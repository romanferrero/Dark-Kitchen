using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
}

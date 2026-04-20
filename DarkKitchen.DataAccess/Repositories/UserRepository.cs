using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
}

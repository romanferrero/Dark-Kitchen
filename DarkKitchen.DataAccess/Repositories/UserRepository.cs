using DarkKitchen.DataAccess.Context;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
}

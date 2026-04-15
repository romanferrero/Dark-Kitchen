using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : Repository<User>(context), IUserRepository
{
    public User? GetByEmail(string email)
    {
        return Context.Users.FirstOrDefault(u => u.Email == email);
    }
}

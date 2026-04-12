using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();
    }

    public User? GetByEmail(string email)
    {
        return context.Users.FirstOrDefault(u => u.Email == email);
    }

    public User? GetById(int id)
    {
        return context.Users.FirstOrDefault(u => u.Id == id);
    }
}

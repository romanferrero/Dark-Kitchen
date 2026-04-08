using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
}

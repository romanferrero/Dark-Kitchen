using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IUserRepository userRepository) : IAuthService
{
    public string Login(string email, string password)
    {
        var user = userRepository.GetByEmail(email);
        if(user == null || user.Password != password)
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        return Guid.NewGuid().ToString();
    }
}

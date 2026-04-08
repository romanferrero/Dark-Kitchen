using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Interfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string Login(string email, string password)
    {
        throw new NotImplementedException();
    }
}

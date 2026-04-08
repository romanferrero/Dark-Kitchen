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
        var user = _userRepository.GetByEmail(email);
        if (user == null || user.Password != password)
        {
            throw new InvalidOperationException("Credenciales inválidas");
        }

        return Guid.NewGuid().ToString();
    }
}

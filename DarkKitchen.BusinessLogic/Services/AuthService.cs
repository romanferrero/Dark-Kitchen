using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService) : IAuthService
{
    public string Login(string email, string password)
    {
        var user = userRepository.GetByEmail(email);
        if(user == null || user.Password != password)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        return jwtTokenService.GenerateToken(user);
    }
}

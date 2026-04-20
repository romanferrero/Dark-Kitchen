using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IRepository<User> userRepository, IJwtTokenService jwtTokenService) : IAuthService
{
    public string Login(string email, string password)
    {
        var user = userRepository.GetAll().FirstOrDefault(u => u.Email == email);
        if(user == null || user.Password != password)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        return jwtTokenService.GenerateToken(user);
    }
}

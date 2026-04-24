using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class AuthService(IRepository<User> userRepository, IJwtTokenService jwtTokenService) : IAuthService
{
    public string Login(string email, string password)
    {
        var user = userRepository.GetAll(u => u.Email == email).FirstOrDefault();
        if(user == null || user.Password != password)
        {
            throw new UnauthorizedAccessException("Credenciales inválidas");
        }

        return jwtTokenService.GenerateToken(user);
    }
}

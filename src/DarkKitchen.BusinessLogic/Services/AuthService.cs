using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class AuthService(IRepository<User> userRepository, ITokenService tokenService) : IAuthService
{
    public string Login(string email, string password)
    {
        var user = userRepository.Get(u => u.Email == email);
        if(user == null || user.Password != password)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return tokenService.GenerateToken(user);
    }
}

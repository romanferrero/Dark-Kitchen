using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}

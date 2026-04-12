using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}

using DarkKitchen.Domain;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    (int UserId, UserRole Role)? ValidateToken(string token);
}

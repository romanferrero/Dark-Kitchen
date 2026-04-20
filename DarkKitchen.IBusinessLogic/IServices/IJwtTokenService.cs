using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    (int UserId, UserRole Role)? ValidateToken(string token);
}

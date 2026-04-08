namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IAuthService
{
    string Login(string email, string password);
}

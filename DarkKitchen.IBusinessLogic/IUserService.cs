namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    void RegisterClient(string firstName, string lastName, string email, string phone, string password);
}

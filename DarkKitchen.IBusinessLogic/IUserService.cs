namespace DarkKitchen.IBusinessLogic;

public interface IUserService
{
    void RegisterClient(string firstName, string lastName, string email, string phone, string password);

    void CreateUser(string firstName, string lastName, string email, string phone, string password,  string role);
}

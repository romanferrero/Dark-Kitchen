namespace DarkKitchen.IBusinessLogic;

public interface IClientService
{
    void RegisterClient(string firstName, string lastName, string email, string phone, string password);
}

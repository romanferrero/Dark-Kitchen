namespace DarkKitchen.BusinessLogic.Interfaces;

public interface IClientService
{
    void RegisterClient(string nombre, string apellido, string email, string telefono, string password);
}

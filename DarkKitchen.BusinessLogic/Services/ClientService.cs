using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class ClientService(IUserRepository userRepository) : IClientService
{
    public void RegisterClient(string nombre, string apellido, string email, string telefono, string password)
    {
        var user = new User
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email,
            Telefono = telefono,
            Password = password,
            Rol = UserRole.Client,
        };

        userRepository.Add(user);
    }
}

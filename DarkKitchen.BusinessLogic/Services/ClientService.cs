using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class ClientService(IUserRepository userRepository) : IClientService
{
    public void RegisterClient(string nombre, string apellido, string email, string telefono, string password)
    {
        if (string.IsNullOrEmpty(nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.");
        }

        if (apellido.Length < 3 || apellido.Length > 25)
        {
            throw new ArgumentException("El apellido debe tener entre 3 y 25 caracteres.");
        }

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

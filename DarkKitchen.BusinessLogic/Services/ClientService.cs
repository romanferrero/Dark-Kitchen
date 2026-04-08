using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class ClientService(IUserRepository userRepository) : IClientService
{
    public void RegisterClient(string nombre, string apellido, string email, string telefono, string password)
    {
        if(string.IsNullOrEmpty(nombre))
        {
            throw new ArgumentException("El nombre no puede estar vacío.");
        }

        if(apellido.Length < 3 || apellido.Length > 25)
        {
            throw new ArgumentException("El apellido debe tener entre 3 y 25 caracteres.");
        }

        if(!email.Contains('@') || !email.Contains('.'))
        {
            throw new ArgumentException("El email no tiene un formato válido.");
        }

        if(password.Length < 15 || password.Length > 25)
        {
            throw new ArgumentException("La contraseña debe tener entre 15 y 25 caracteres.");
        }

        if(!password.Any(char.IsUpper))
        {
            throw new ArgumentException("La contraseña debe contener al menos una mayúscula.");
        }

        if(!password.Any(char.IsLower))
        {
            throw new ArgumentException("La contraseña debe contener al menos una minúscula.");
        }

        if(!password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation))
        {
            throw new ArgumentException("La contraseña debe contener al menos un símbolo.");
        }

        if(!password.Any(char.IsDigit))
        {
            throw new ArgumentException("La contraseña debe contener al menos un número.");
        }

        if(HasNumericSequence(password))
        {
            throw new ArgumentException("La contraseña no puede contener secuencias numéricas.");
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

    private static bool HasNumericSequence(string password)
    {
        for(var i = 0; i < password.Length - 2; i++)
        {
            if(char.IsDigit(password[i]) &&
                char.IsDigit(password[i + 1]) &&
                char.IsDigit(password[i + 2]) &&
                password[i + 1] - password[i] == 1 &&
                password[i + 2] - password[i + 1] == 1)
            {
                return true;
            }
        }

        return false;
    }
}

using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.Domain;

namespace DarkKitchen.BusinessLogic.Services;

public class ClientService(IUserRepository userRepository) : IClientService
{
    public void RegisterClient(string firstName, string lastName, string email, string phone, string password)
    {
        if(string.IsNullOrEmpty(firstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }

        if(lastName.Length < 3 || lastName.Length > 25)
        {
            throw new ArgumentException("Last name must be between 3 and 25 characters.");
        }

        if(!email.Contains('@') || !email.Contains('.'))
        {
            throw new ArgumentException("Email format is invalid.");
        }

        if(password.Length < 15 || password.Length > 25)
        {
            throw new ArgumentException("Password must be between 15 and 25 characters.");
        }

        if(!password.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }

        if(!password.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }

        if(!password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation))
        {
            throw new ArgumentException("Password must contain at least one symbol.");
        }

        if(!password.Any(char.IsDigit))
        {
            throw new ArgumentException("Password must contain at least one digit.");
        }

        if(HasNumericSequence(password))
        {
            throw new ArgumentException("Password cannot contain numeric sequences.");
        }

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = UserRole.Client,
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

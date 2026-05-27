using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class User
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _phone = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public UserRole Role { get; set; }

    public static User CreateClient(string firstName, string lastName, string email, string phone, string password)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = UserRole.Client
        };
    }

    public static User CreateInternal(string firstName, string lastName, string email,
        string phone, string password, string role)
    {
        if(role != "Admin" && role != "Dispatcher")
        {
            throw new ArgumentException("Role must be 'Admin' or 'Dispatcher'.");
        }

        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = role == "Admin" ? UserRole.Admin : UserRole.Dispatcher
        };
    }

    public void Update(string firstName, string lastName, string email, string phone, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Password = password;
    }

    public int Id { get; set; }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("First name cannot be empty.");
            }

            _firstName = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Last name cannot be empty.");
            }

            if(value.Length < 3 || value.Length > 25)
            {
                throw new ArgumentException("Last name must be between 3 and 25 characters.");
            }

            _lastName = value;
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            if(!value.Contains('@') || !value.Contains('.'))
            {
                throw new ArgumentException("Email format is invalid.");
            }

            _email = value;
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Phone cannot be empty.");
            }

            _phone = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            if(value.Length < 15 || value.Length > 25)
            {
                throw new ArgumentException("Password must be between 15 and 25 characters.");
            }

            if(!value.Any(char.IsUpper))
            {
                throw new ArgumentException("Password must contain at least one uppercase letter.");
            }

            if(!value.Any(char.IsLower))
            {
                throw new ArgumentException("Password must contain at least one lowercase letter.");
            }

            if(!value.Any(char.IsSymbol) && !value.Any(char.IsPunctuation))
            {
                throw new ArgumentException("Password must contain at least one symbol.");
            }

            if(!value.Any(char.IsDigit))
            {
                throw new ArgumentException("Password must contain at least one digit.");
            }

            if(HasNumericSequence(value))
            {
                throw new ArgumentException("Password cannot contain numeric sequences.");
            }

            _password = value;
        }
    }

    private static bool HasNumericSequence(string password)
    {
        for(var i = 0; i < password.Length - 2; i++)
        {
            if(!char.IsDigit(password[i]) || !char.IsDigit(password[i + 1]) || !char.IsDigit(password[i + 2]))
            {
                continue;
            }

            var firstToSecond = password[i + 1] - password[i];
            var secondToThird = password[i + 2] - password[i + 1];

            var isAscending = firstToSecond == 1 && secondToThird == 1;
            var isDescending = firstToSecond == -1 && secondToThird == -1;

            if(isAscending || isDescending)
            {
                return true;
            }
        }

        return false;
    }
}

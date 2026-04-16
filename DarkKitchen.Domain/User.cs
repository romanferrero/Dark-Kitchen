namespace DarkKitchen.Domain;

public class User
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;

    public static User CreateClient(string firstName, string lastName, string email, string phone, string password)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = UserRole.Client,
        };
    }

    public static User CreateInternal(string firstName, string lastName, string email,
        string phone, string password, string role)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = UserRole.Admin,
        };
    }

    public int Id { get; set; }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if(string.IsNullOrEmpty(value))
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
            if(!value.Contains('@') || !value.Contains('.'))
            {
                throw new ArgumentException("Email format is invalid.");
            }

            _email = value;
        }
    }

    public string Phone { get; set; } = string.Empty;

    public string Password
    {
        get => _password;
        set
        {
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

    public UserRole Role { get; set; }

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

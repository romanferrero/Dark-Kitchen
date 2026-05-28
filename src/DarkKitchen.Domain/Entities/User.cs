using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public class User
{
    private const int MinLastNameLength = 3;
    private const int MaxLastNameLength = 25;
    private const int MinPasswordLength = 15;
    private const int MaxPasswordLength = 25;
    private const int NumericSequenceLength = 3;

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
        string phone, string password, UserRole role)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = role
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
            ValidateNonEmpty(value, "First name");
            _firstName = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            ValidateLastName(value);
            _lastName = value;
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            ValidateEmail(value);
            _email = value;
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            ValidateNonEmpty(value, "Phone");
            _phone = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            ValidatePassword(value);
            _password = value;
        }
    }

    private static void ValidateNonEmpty(string value, string fieldName)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} cannot be empty.");
        }
    }

    private static void ValidateLastName(string value)
    {
        ValidateNonEmpty(value, "Last name");

        var isInvalidLength = value.Length < MinLastNameLength || value.Length > MaxLastNameLength;
        if(isInvalidLength)
        {
            throw new ArgumentException($"Last name must be between {MinLastNameLength} and {MaxLastNameLength} characters.");
        }
    }

    private static void ValidateEmail(string value)
    {
        ValidateNonEmpty(value, "Email");

        var hasInvalidFormat = !value.Contains('@') || !value.Contains('.');
        if(hasInvalidFormat)
        {
            throw new ArgumentException("Email format is invalid.");
        }
    }

    private static void ValidatePassword(string value)
    {
        ValidateNonEmpty(value, "Password");

        var isInvalidLength = value.Length < MinPasswordLength || value.Length > MaxPasswordLength;
        if(isInvalidLength)
        {
            throw new ArgumentException($"Password must be between {MinPasswordLength} and {MaxPasswordLength} characters.");
        }

        if(!value.Any(char.IsUpper))
        {
            throw new ArgumentException("Password must contain at least one uppercase letter.");
        }

        if(!value.Any(char.IsLower))
        {
            throw new ArgumentException("Password must contain at least one lowercase letter.");
        }

        var hasSymbol = value.Any(char.IsSymbol) || value.Any(char.IsPunctuation);
        if(!hasSymbol)
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
    }

    private static bool HasNumericSequence(string password)
    {
        for(var i = 0; i <= password.Length - NumericSequenceLength; i++)
        {
            var allDigits = char.IsDigit(password[i])
                            && char.IsDigit(password[i + 1])
                            && char.IsDigit(password[i + 2]);

            if(!allDigits)
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

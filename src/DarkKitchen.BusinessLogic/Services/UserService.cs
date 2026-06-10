using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IBusinessLogic.IValidators;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class UserService(
    IRepository<User> userRepository,
    IPhoneValidator phoneValidator,
    IPasswordHasher passwordHasher) : IUserService
{
    public RegisterClientExitDto RegisterClient(RegisterClientEntryDto dto)
    {
        ValidatePhone(dto.Phone);
        ValidateEmailUnique(dto.Email);

        var passwordHash = HashValidatedPassword(dto.Password);

        var user = User.CreateClient(dto.FirstName, dto.LastName, dto.Email, dto.Phone, passwordHash);

        userRepository.Add(user);

        return ToRegisterClientExitDto(user);
    }

    public UserExitDto CreateUser(CreateUserEntryDto dto)
    {
        ValidatePhone(dto.Phone);
        ValidateEmailUnique(dto.Email);

        var role = ParseInternalRole(dto.Role);

        var passwordHash = HashValidatedPassword(dto.Password);

        var user = User.CreateInternal(dto.FirstName, dto.LastName, dto.Email,
            dto.Phone, passwordHash, role);

        userRepository.Add(user);

        return ToUserExitDto(user);
    }

    private static UserRole ParseInternalRole(string role)
    {
        if(!Enum.TryParse<UserRole>(role, out var parsed) || parsed == UserRole.Client)
        {
            throw new ArgumentException("Role must be 'Admin' or 'Dispatcher'.");
        }

        return parsed;
    }

    public void DeleteUser(int userId, int currentUserId)
    {
        if(userId == currentUserId)
        {
            throw new ArgumentException("A user cannot delete themselves.");
        }

        var user = userRepository.Get(u => u.Id == userId)
                   ?? throw new KeyNotFoundException($"User with id '{userId}' not found.");

        userRepository.Delete(u => u.Id == user.Id);
    }

    public UserExitDto UpdateUser(int id, UpdateUserEntryDto dto, int currentUserId)
    {
        if(id == currentUserId)
        {
            throw new ArgumentException("A user cannot modify themselves.");
        }

        var user = userRepository.Get(u => u.Id == id)
                   ?? throw new KeyNotFoundException($"User with id '{id}' not found.");

        ValidatePhone(dto.Phone);

        if(!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            ValidateEmailUnique(dto.Email);
        }

        var passwordHash = HashValidatedPassword(dto.Password);

        user.Update(dto.FirstName, dto.LastName, dto.Email, dto.Phone, passwordHash);
        userRepository.Update(user);

        return ToUserExitDto(user);
    }

    public List<UserExitDto> GetUsers(string? firstName, string? lastName)
    {
        var users = userRepository.GetAll();

        if(!string.IsNullOrWhiteSpace(firstName))
        {
            users = users
                .Where(u => u.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if(!string.IsNullOrWhiteSpace(lastName))
        {
            users = users
                .Where(u => u.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return users.Select(ToUserExitDto).ToList();
    }

    private void ValidateEmailUnique(string email)
    {
        if(userRepository.Exists(u => u.Email == email))
        {
            throw new InvalidOperationException($"A user with email '{email}' already exists.");
        }
    }

    private string HashValidatedPassword(string password)
    {
        User.ValidatePassword(password);
        return passwordHasher.Hash(password);
    }

    private void ValidatePhone(string phone)
    {
        if(!phoneValidator.IsValid(phone))
        {
            throw new ArgumentException(phoneValidator.ErrorMessage);
        }
    }

    private static RegisterClientExitDto ToRegisterClientExitDto(User user)
    {
        return new RegisterClientExitDto(
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone);
    }

    private static UserExitDto ToUserExitDto(User user)
    {
        return new UserExitDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString()
        };
    }
}

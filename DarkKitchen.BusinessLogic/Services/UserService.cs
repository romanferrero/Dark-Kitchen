using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IRepository<User> userRepository, IPhoneValidator phoneValidator) : IUserService
{
    private static GetUsersDto ToDto(User user)
    {
        return new GetUsersDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.ToString()
        };
    }

    public void RegisterClient(string firstName, string lastName, string email, string phone, string password)
    {
        ValidatePhone(phone);
        ValidateEmailUnique(email);

        var user = User.CreateClient(firstName, lastName, email, phone, password);

        userRepository.Add(user);
    }

    public void CreateUser(string firstName, string lastName, string email, string phone, string password, string role)
    {
        ValidatePhone(phone);
        ValidateEmailUnique(email);

        var user = User.CreateInternal(firstName, lastName, email, phone, password, role);

        userRepository.Add(user);
    }

    public void DeleteUser(int userId, int currentUserId)
    {
        if(userId == currentUserId)
        {
            throw new ArgumentException("A user cannot delete themselves.");
        }

        var user = userRepository.GetAll(u => u.Id == userId).FirstOrDefault()
                   ?? throw new KeyNotFoundException($"User with id '{userId}' not found.");

        userRepository.Delete(u => u.Id == user.Id);
    }

    public void UpdateUser(int id, string firstName, string lastName, string email, string phone, string password,
        int currentUserId)
    {
        if(id == currentUserId)
        {
            throw new ArgumentException("A user cannot modify themselves.");
        }

        var user = userRepository.GetAll(u => u.Id == id).FirstOrDefault()
                   ?? throw new KeyNotFoundException($"User with id '{id}' not found.");

        ValidatePhone(phone);

        if(!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            ValidateEmailUnique(email);
        }

        user.Update(firstName, lastName, email, phone, password);

        userRepository.Update(user);
    }

    public List<GetUsersDto> GetUsers(string? firstName, string? lastName)
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

        return users.Select(ToDto).ToList();
    }

    private void ValidateEmailUnique(string email)
    {
        var existing = userRepository.GetAll(u => u.Email == email);

        if(existing.Any())
        {
            throw new InvalidOperationException($"A user with email '{email}' already exists.");
        }
    }

    private void ValidatePhone(string phone)
    {
        if(!phoneValidator.IsValid(phone))
        {
            throw new ArgumentException(phoneValidator.ErrorMessage);
        }
    }
}

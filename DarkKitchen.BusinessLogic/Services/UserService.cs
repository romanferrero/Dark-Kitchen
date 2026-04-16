using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IRepository<User> userRepository) : IUserService
{
    public void RegisterClient(string firstName, string lastName, string email, string phone, string password)
    {
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

    public void CreateUser(string firstName, string lastName,
        string email, string phone, string password, string role)
    {
        if(role != "Admin" && role != "Dispatcher")
        {
            throw new ArgumentException();
        }

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Password = password,
            Role = role == "Admin" ? UserRole.Admin : UserRole.Dispatcher
        };

        userRepository.Add(user);
    }

    public void DeleteUser(int userId, int currentUserId)
    {
        if(userId == currentUserId)
        {
            throw new ArgumentException();
        }

        var user = userRepository.GetAll().FirstOrDefault(u => u.Id == userId);
        if(user == null)
        {
            throw new ArgumentException();
        }

        userRepository.Delete(u => u.Id == userId);
    }

    public void UpdateUser(int id, string firstName, string lastName, string email,
        string phone, string password, int currentUserId)
    {
        if(id == currentUserId)
        {
            throw new ArgumentException();
        }

        var user = userRepository.GetAll().FirstOrDefault(u => u.Id == id);
        if(user == null)
        {
            throw new ArgumentException();
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.Phone = phone;
        user.Password = password;

        userRepository.Update(user);
    }

    public List<GetUsersDto> GetUsers(string? firstName, string? lastName)
    {
        var users = userRepository.GetAll();

        return users.Select(u => new GetUsersDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.Phone,
            Role = u.Role.ToString()
        }).ToList();
    }
}

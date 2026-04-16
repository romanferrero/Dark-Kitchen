using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;

namespace DarkKitchen.BusinessLogic.Services;

public class UserService(IRepository<User> userRepository) : IUserService
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

        if(!string.IsNullOrWhiteSpace(firstName))
        {
            users = users.Where(u => u.FirstName == firstName).ToList();
        }

        if(!string.IsNullOrWhiteSpace(lastName))
        {
            users = users.Where(u => u.LastName == lastName).ToList();
        }

        return users.Select(ToDto).ToList();
    }
}

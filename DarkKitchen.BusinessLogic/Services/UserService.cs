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
        throw new NotImplementedException();
    }

    public void UpdateUser(int id, string firstName, string lastName, string email,
        string phone, string password, int currentUserId)
    {
        throw new NotImplementedException();
    }

    public List<GetUsersDto> GetUsers(string? firstName, string? lastName)
    {
        throw new NotImplementedException();
    }
}

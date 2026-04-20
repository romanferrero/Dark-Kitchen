using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IUserService
{
    void RegisterClient(string firstName, string lastName, string email, string phone, string password);

    void CreateUser(string firstName, string lastName, string email, string phone, string password, string role);

    void DeleteUser(int id, int currentUserId);

    void UpdateUser(int id, string firstName, string lastName, string email, string phone, string password,
        int currentUserId);

    List<GetUsersExitDTO> GetUsers(string? firstName, string? lastName);
}

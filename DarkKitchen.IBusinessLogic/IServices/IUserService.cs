using DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IUserService
{
    RegisterClientExitDto RegisterClient(RegisterClientEntryDto dto);
    UserExitDto CreateUser(CreateUserEntryDto dto);
    void DeleteUser(int id, int currentUserId);
    UserExitDto UpdateUser(int id, UpdateUserEntryDto dto, int currentUserId);
    List<UserExitDto> GetUsers(string? firstName, string? lastName);
}

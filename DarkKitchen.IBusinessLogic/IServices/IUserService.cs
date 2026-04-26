using DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IUserService
{
    RegisterClientExitDTO RegisterClient(RegisterClientEntryDTO dto);
    UserExitDto CreateUser(CreateUserEntryDto dto, int currentUserId);
    void DeleteUser(int id, int currentUserId);
    UserExitDto UpdateUser(int id, UpdateUserEntryDto dto, int currentUserId);
    List<UserExitDto> GetUsers(string? firstName, string? lastName);
}

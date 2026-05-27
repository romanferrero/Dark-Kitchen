namespace DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;

public record CreateUserEntryDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password,
    string Role
);

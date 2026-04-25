namespace DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;

public record RegisterClientEntryDTO(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password
);

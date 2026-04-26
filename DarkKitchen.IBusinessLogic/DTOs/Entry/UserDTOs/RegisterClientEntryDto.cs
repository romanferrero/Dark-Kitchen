namespace DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;

public record RegisterClientEntryDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password
);

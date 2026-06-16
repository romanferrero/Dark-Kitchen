namespace DarkKitchen.IBusinessLogic.DTOs.Entry;

public record UpdateUserEntryDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password
);

namespace DarkKitchen.IBusinessLogic.DTOs.Entry;

public record RegisterClientEntryDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Password
);

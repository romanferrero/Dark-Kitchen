namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public record RegisterClientExitDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone
);

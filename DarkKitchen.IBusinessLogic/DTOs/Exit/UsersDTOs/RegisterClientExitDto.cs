namespace DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;

public record RegisterClientExitDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone
);

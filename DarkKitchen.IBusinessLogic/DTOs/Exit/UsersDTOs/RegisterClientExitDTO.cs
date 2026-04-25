namespace DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;

public record RegisterClientExitDTO(
    string FirstName,
    string LastName,
    string Email,
    string Phone
);

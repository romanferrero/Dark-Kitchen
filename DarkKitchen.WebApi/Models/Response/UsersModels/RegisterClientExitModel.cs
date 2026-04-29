namespace DarkKitchen.WebApi.Models.Response.UsersModels;

public record RegisterClientExitModel(
    string FirstName,
    string LastName,
    string Email,
    string Phone
);

using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.WebApi.Models.Request.UserModels;
using DarkKitchen.WebApi.Models.Response.UsersModels;

namespace DarkKitchen.WebApi.Mappers;

internal static class UserMapper
{
    internal static RegisterClientEntryDto ToDto(RegisterClientRequestModel request)
    {
        return new RegisterClientEntryDto(request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password);
    }

    internal static RegisterClientExitModel ToResponse(RegisterClientExitDto dto)
    {
        return new RegisterClientExitModel(dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Phone);
    }

    internal static CreateUserEntryDto ToDto(CreateUserRequestModel request)
    {
        return new CreateUserEntryDto(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password,
            request.Role);
    }

    internal static UpdateUserEntryDto ToDto(UpdateUserRequestModel request)
    {
        return new UpdateUserEntryDto(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password);
    }

    internal static UserResponseModel ToResponse(UserExitDto user)
    {
        return new UserResponseModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role
        };
    }
}

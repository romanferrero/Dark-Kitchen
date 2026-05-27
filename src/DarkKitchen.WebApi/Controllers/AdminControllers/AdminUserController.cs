using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.UserModels;
using DarkKitchen.WebApi.Models.Response.UsersModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.AdminControllers;

[ApiController]
[Route("api/users")]
public sealed class AdminUserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult CreateUser(CreateUserRequestModel request)
    {
        var result = userService.CreateUser(ToDto(request));

        return Created(string.Empty, ToResponse(result));
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult DeleteUser(int id)
    {
        userService.DeleteUser(id, GetCurrentUserId());
        return NoContent();
    }

    [HttpPut("{id}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult UpdateUser(int id, UpdateUserRequestModel request)
    {
        var result = userService.UpdateUser(id, ToDto(request), GetCurrentUserId());

        return Ok(ToResponse(result));
    }

    [HttpGet]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult GetUsers(
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null)
    {
        var users = userService.GetUsers(firstName, lastName);
        return Ok(users.Select(ToResponse).ToList());
    }

    private int GetCurrentUserId()
    {
        return (int)HttpContext.Items["UserId"]!;
    }

    private static CreateUserEntryDto ToDto(CreateUserRequestModel request)
    {
        return new CreateUserEntryDto(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password,
            request.Role);
    }

    private static UpdateUserEntryDto ToDto(UpdateUserRequestModel request)
    {
        return new UpdateUserEntryDto(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password);
    }

    private static UserResponseModel ToResponse(UserExitDto user)
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

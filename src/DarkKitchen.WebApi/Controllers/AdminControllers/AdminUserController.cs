using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.AdminControllers;

[ApiController]
[Route("api/users")]
public sealed class AdminUserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(Permission.ManageInternalUsers)]
    public IActionResult CreateUser(CreateUserRequestModel request)
    {
        var result = userService.CreateUser(UserMapper.ToDto(request));

        return Created(string.Empty, UserMapper.ToResponse(result));
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter(Permission.ManageInternalUsers)]
    public IActionResult DeleteUser(int id)
    {
        userService.DeleteUser(id, GetCurrentUserId());
        return NoContent();
    }

    [HttpPut("{id}")]
    [AuthorizationFilter(Permission.ManageInternalUsers)]
    public IActionResult UpdateUser(int id, UpdateUserRequestModel request)
    {
        var result = userService.UpdateUser(id, UserMapper.ToDto(request), GetCurrentUserId());

        return Ok(UserMapper.ToResponse(result));
    }

    [HttpGet]
    [AuthorizationFilter(Permission.ManageInternalUsers)]
    public IActionResult GetUsers(
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null)
    {
        var users = userService.GetUsers(firstName, lastName);
        return Ok(users.Select(UserMapper.ToResponse).ToList());
    }

    private int GetCurrentUserId()
    {
        return (int)HttpContext.Items["UserId"]!;
    }
}

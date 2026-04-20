using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.AdminControllers;

[ApiController]
[Route("api/admin/users")]
public class AdminUserController(IUserService userService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        return (int)HttpContext.Items["UserId"]!;
    }

    [HttpPost]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult CreateUser(CreateUserRequestModel request)
    {
        userService.CreateUser(
            request.FirstName, request.LastName, request.Email,
            request.Phone, request.Password, request.Role);

        return Created(string.Empty, null);
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult DeleteUser(int id)
    {
        userService.DeleteUser(id, GetCurrentUserId());
        return Ok();
    }

    [HttpPut("{id}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult UpdateUser(int id, UpdateUserRequestModel request)
    {
        userService.UpdateUser(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password,
            GetCurrentUserId());

        return Ok();
    }

    [HttpGet]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult GetUsers(
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null)
    {
        var users = userService.GetUsers(firstName, lastName);
        return Ok(users);
    }
}

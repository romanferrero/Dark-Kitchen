using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/admin/users")]
public class AdminUserController(IUserService userService) : ControllerBase
{
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
        userService.DeleteUser(id, 0);
        return Ok();
    }

    [HttpPut]
    public IActionResult UpdateUser(int id, UpdateUserRequestModel request)
    {
        userService.UpdateUser(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password,
            0);

        return Ok();
    }
}

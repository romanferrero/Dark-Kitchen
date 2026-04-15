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
    public IActionResult CreateUser(CreateUserRequestModel request)
    {
        return Created(string.Empty, null);
    }
}

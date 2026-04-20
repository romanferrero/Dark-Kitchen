using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.UsersControllers;

[ApiController]
[Route("api/clients")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public IActionResult RegisterClient(RegisterClientRequestModel request)
    {
        userService.RegisterClient(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password);

        return Created(string.Empty, null);
    }
}

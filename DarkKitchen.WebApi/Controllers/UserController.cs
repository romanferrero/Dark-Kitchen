using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/clients")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public IActionResult RegisterClient(RegisterClientRequestModel request)
    {
        try
        {
            userService.RegisterClient(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Phone,
                request.Password);

            return Created(string.Empty, null);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

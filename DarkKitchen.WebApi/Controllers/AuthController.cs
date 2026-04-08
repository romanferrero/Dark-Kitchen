using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequestModel request)
    {
        try
        {
            var token = authService.Login(request.Email, request.Password);
            return Ok(token);
        }
        catch(InvalidOperationException)
        {
            return Unauthorized();
        }
    }
}

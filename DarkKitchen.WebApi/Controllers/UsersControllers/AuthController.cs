using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.UsersControllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequestModel request)
    {
        var token = authService.Login(request.Email, request.Password);
        return Ok(token);
    }
}

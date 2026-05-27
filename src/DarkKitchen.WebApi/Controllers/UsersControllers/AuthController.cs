using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.UsersControllers;

[ApiController]
[Route("api/sessions")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost]
    public IActionResult Login(LoginRequestModel request)
    {
        var token = authService.Login(request.Email, request.Password);
        return Created(string.Empty, token);
    }
}

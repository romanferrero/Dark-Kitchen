using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models.Request.UserModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.UsersControllers;

[ApiController]
[Route("api/clients")]
public sealed class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public IActionResult RegisterClient(RegisterClientRequestModel request)
    {
        var registerClientExitDto = userService.RegisterClient(UserMapper.ToDto(request));

        return Created(string.Empty, UserMapper.ToResponse(registerClientExitDto));
    }
}

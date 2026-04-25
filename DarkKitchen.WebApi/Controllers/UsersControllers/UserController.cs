using DarkKitchen.IBusinessLogic.DTOs.Entry.UserDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.UsersDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Models.Request.UserModels;
using DarkKitchen.WebApi.Models.Response.UsersModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.UsersControllers;

[ApiController]
[Route("api/clients")]
public sealed class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public IActionResult RegisterClient(RegisterClientRequestModel request)
    {
        var registerClientExitDto = userService.RegisterClient(ToDto(request));

        return Created(string.Empty, ToResponse(registerClientExitDto));
    }

    private static RegisterClientEntryDTO ToDto(RegisterClientRequestModel request)
    {
        return new RegisterClientEntryDTO(request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Password);
    }

    private static RegisterClientExitModel ToResponse(RegisterClientExitDTO dto)
    {
        return new RegisterClientExitModel(dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Phone);
    }
}

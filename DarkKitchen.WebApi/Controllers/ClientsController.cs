using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public IActionResult RegisterClient(RegisterClientRequestModel request)
    {
        try
        {
            clientService.RegisterClient(
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

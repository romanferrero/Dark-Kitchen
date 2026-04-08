using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpPost]
    public IActionResult RegisterClient(RegisterClientRequestModel request)
    {
        try
        {
            clientService.RegisterClient(
                request.Nombre,
                request.Apellido,
                request.Email,
                request.Telefono,
                request.Password);

            return Created(string.Empty, null);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

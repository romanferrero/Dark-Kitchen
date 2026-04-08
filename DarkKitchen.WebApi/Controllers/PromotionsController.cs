using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(IPromotionService promService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreatePromotion(CreatePromotionRequestModel request)
    {
        try
        {
            var result = promService.CreatePromotion(
                request.Name,
                request.Discount,
                request.DateFrom,
                request.DateTo);

            return CreatedAtAction(nameof(CreatePromotion), null, result);
        }
        catch(ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpGet]
    public IActionResult GetPromotions(
    [FromQuery] string? date,
    [FromQuery] string? line,
    [FromQuery] string? product)
    {
        var result = promService.GetPromotions(date, line, product);
        return Ok(result);
    }
}

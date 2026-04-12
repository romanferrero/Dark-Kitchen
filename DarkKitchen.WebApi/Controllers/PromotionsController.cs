using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(IPromotionService promService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdatePromotion(int id, UpdatePromotionRequestModel request)
    {
        try
        {
            var result = promService.UpdatePromotion(
                id,
                request.Name,
                request.Discount,
                request.DateFrom,
                request.DateTo);

            return Ok(result);
        }
        catch(ArgumentException)
        {
            return BadRequest();
        }
        catch(KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:int}/products")]
    [Authorize(Roles = "Admin")]
    public IActionResult AddProduct(int id, AddProductToPromotionRequestModel request)
    {
        try
        {
            var result = promService.AddProduct(id, request.ProductCode);
            return Ok(result);
        }
        catch(KeyNotFoundException)
        {
            return NotFound();
        }
        catch(InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:int}/products/{productCode}")]
    [Authorize(Roles = "Admin")]
    public IActionResult RemoveProduct(int id, string productCode)
    {
        try
        {
            var result = promService.RemoveProduct(id, productCode);
            return Ok(result);
        }
        catch(KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [Authorize(Roles = "Client,Admin")]
    public IActionResult GetPromotions(
        [FromQuery] string? date,
        [FromQuery] string? line,
        [FromQuery] string? product)
    {
        DateOnly? parsedDate = null;

        if(!string.IsNullOrWhiteSpace(date) && DateOnly.TryParse(date, out var d))
        {
            parsedDate = d;
        }

        var promotions = promService.GetPromotions(parsedDate, line, product);

        var response = promotions.Select(p => new PromotionResponseModel
        {
            Id = p.Id,
            Name = p.Name,
            DiscountPercentage = p.DiscountPercentage,
            DateFrom = p.DateFrom,
            DateTo = p.DateTo,
            Products = p.Products.Select(pr => pr.Code).ToList(),
        }).ToList();

        return Ok(response);
    }
}

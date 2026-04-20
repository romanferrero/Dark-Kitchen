using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.PromotionsModels;
using DarkKitchen.WebApi.Models.Response.PromotionsModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.PromotionsControllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(IPromotionService promService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult CreatePromotion(CreatePromotionRequestModel request)
    {
        var result = promService.CreatePromotion(
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);

        return CreatedAtAction(nameof(CreatePromotion), null, result);
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult UpdatePromotion(int id, UpdatePromotionRequestModel request)
    {
        var result = promService.UpdatePromotion(
            id,
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);

        return Ok(result);
    }

    [HttpPost("{id:int}/products")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult AddProduct(int id, AddProductToPromotionRequestModel request)
    {
        var result = promService.AddProduct(id, request.ProductCode);
        return Ok(result);
    }

    [HttpDelete("{id:int}/products/{productCode}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult RemoveProduct(int id, string productCode)
    {
        var result = promService.RemoveProduct(id, productCode);
        return Ok(result);
    }

    [HttpGet]
    [AuthorizationFilter(UserRole.Client, UserRole.Admin)]
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

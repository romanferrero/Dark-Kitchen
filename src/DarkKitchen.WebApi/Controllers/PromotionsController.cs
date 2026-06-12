using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.PromotionsModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(IPromotionService promService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilterAttribute(Permission.ManagePromotions)]
    public IActionResult CreatePromotion(CreatePromotionRequestModel request)
    {
        var responsibleUser = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var promotion = promService.CreatePromotion(PromotionMapper.ToDto(request), responsibleUser);

        return Created(string.Empty, PromotionMapper.ToResponse(promotion));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilterAttribute(Permission.ManagePromotions)]
    public IActionResult UpdatePromotion(int id, UpdatePromotionRequestModel request)
    {
        var responsibleUser = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var promotion = promService.UpdatePromotion(PromotionMapper.ToDto(id, request), responsibleUser);

        return Ok(PromotionMapper.ToResponse(promotion));
    }

    [HttpPost("{id:int}/products")]
    [AuthorizationFilterAttribute(Permission.ManagePromotionProducts)]
    public IActionResult AddProduct(int id, AddProductToPromotionRequestModel request)
    {
        var responsibleUser = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        var product = promService.AddProduct(id, request.ProductCode, responsibleUser);

        return Ok(ProductMapper.ToResponse(product));
    }

    [HttpDelete("{id:int}/products")]
    [AuthorizationFilterAttribute(Permission.ManagePromotionProducts)]
    public IActionResult RemoveProduct(int id, [FromQuery] string code)
    {
        var responsibleUser = HttpContext.Items["UserId"]?.ToString() ?? string.Empty;
        promService.RemoveProduct(id, code, responsibleUser);

        return NoContent();
    }

    [HttpGet]
    [AuthorizationFilterAttribute(Permission.ViewPromotions)]
    public IActionResult GetPromotions([FromQuery] GetPromotionsQueryModel query)
    {
        DateOnly? parsedDate = null;

        if(!string.IsNullOrWhiteSpace(query.Date) && DateOnly.TryParse(query.Date, out var d))
        {
            parsedDate = d;
        }

        var promotions = promService.GetPromotions(parsedDate, query.Line, query.Product)
            .Select(PromotionMapper.ToResponse)
            .ToList();

        return Ok(PagedResult.Create(promotions, query.PageNumber, query.PageSize));
    }
}

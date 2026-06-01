using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.PromotionsModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.PromotionsControllers;

[ApiController]
[Route("api/promotions")]
public class PromotionsController(IPromotionService promService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(Permission.ManagePromotions)]
    public IActionResult CreatePromotion(CreatePromotionRequestModel request)
    {
        var promotion = promService.CreatePromotion(PromotionMapper.ToDto(request));

        return Created(string.Empty, PromotionMapper.ToResponse(promotion));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(Permission.ManagePromotions)]
    public IActionResult UpdatePromotion(int id, UpdatePromotionRequestModel request)
    {
        var promotion = promService.UpdatePromotion(PromotionMapper.ToDto(id, request));

        return Ok(PromotionMapper.ToResponse(promotion));
    }

    [HttpPost("{id:int}/products")]
    [AuthorizationFilter(Permission.ManagePromotionProducts)]
    public IActionResult AddProduct(int id, AddProductToPromotionRequestModel request)
    {
        var product = promService.AddProduct(id, request.ProductCode);

        return Ok(ProductMapper.ToResponse(product));
    }

    [HttpDelete("{id:int}/products")]
    [AuthorizationFilter(Permission.ManagePromotionProducts)]
    public IActionResult RemoveProduct(int id, [FromQuery] string code)
    {
        promService.RemoveProduct(id, code);

        return NoContent();
    }

    [HttpGet]
    [AuthorizationFilter(Permission.ViewPromotions)]
    public IActionResult GetPromotions([FromQuery] GetPromotionsQueryModel query)
    {
        DateOnly? parsedDate = null;

        if(!string.IsNullOrWhiteSpace(query.Date) && DateOnly.TryParse(query.Date, out var d))
        {
            parsedDate = d;
        }

        var promotions = promService.GetPromotions(parsedDate, query.Line, query.Product);

        return Ok(promotions.Select(PromotionMapper.ToResponse).ToList());
    }
}

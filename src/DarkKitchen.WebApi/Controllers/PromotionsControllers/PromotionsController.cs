using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.PromotionsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;
using DarkKitchen.WebApi.Models.Response.PromotionsModels;
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
        var promotion = promService.CreatePromotion(ToDto(request));

        return Created(string.Empty, ToResponse(promotion));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(Permission.ManagePromotions)]
    public IActionResult UpdatePromotion(int id, UpdatePromotionRequestModel request)
    {
        var promotion = promService.UpdatePromotion(ToDto(id, request));

        return Ok(ToResponse(promotion));
    }

    [HttpPost("{id:int}/products")]
    [AuthorizationFilter(Permission.ManagePromotionProducts)]
    public IActionResult AddProduct(int id, AddProductToPromotionRequestModel request)
    {
        var product = promService.AddProduct(id, request.ProductCode);

        return Ok(ToResponse(product));
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

        return Ok(promotions.Select(ToResponse).ToList());
    }

    private static CreatePromotionEntryDto ToDto(CreatePromotionRequestModel request)
    {
        return new CreatePromotionEntryDto(
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);
    }

    private static UpdatePromotionEntryDto ToDto(int id, UpdatePromotionRequestModel request)
    {
        return new UpdatePromotionEntryDto(
            id,
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);
    }

    private static PromotionResponseModel ToResponse(PromotionExitDto promotion)
    {
        return new PromotionResponseModel
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            DateFrom = promotion.DateFrom,
            DateTo = promotion.DateTo,
            Products = promotion.Products
        };
    }

    private static ProductResponseModel ToResponse(ProductExitDto product)
    {
        return new ProductResponseModel
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Line = product.Line,
            Category = product.Category,
            ImageUrls = product.ImageUrls
        };
    }
}

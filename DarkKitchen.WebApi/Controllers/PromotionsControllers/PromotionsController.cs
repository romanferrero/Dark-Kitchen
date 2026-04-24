using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
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
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult CreatePromotion(CreatePromotionRequestModel request)
    {
        var promotion = promService.CreatePromotion(
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);

        return Created(string.Empty, ToResponse(promotion));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult UpdatePromotion(int id, UpdatePromotionRequestModel request)
    {
        var promotion = promService.UpdatePromotion(
            id,
            request.Name,
            request.Discount,
            request.DateFrom,
            request.DateTo);

        return Ok(ToResponse(promotion));
    }

    [HttpPost("{id:int}/products")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult AddProduct(int id, AddProductToPromotionRequestModel request)
    {
        var product = promService.AddProduct(id, request.ProductCode);
        return Ok(ToResponse(product));
    }

    [HttpDelete("{id:int}/products/{productCode}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult RemoveProduct(int id, string productCode)
    {
        var product = promService.RemoveProduct(id, productCode);
        return Ok(ToResponse(product));
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

        return Ok(promotions.Select(ToResponse).ToList());
    }

    private static PromotionResponseModel ToResponse(PromotionExitDTO promotion)
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

    private static ProductResponseModel ToResponse(ProductExitDTO product)
    {
        return new ProductResponseModel
        {
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Line = product.Line,
            Category = product.Category,
            ImageUrls = product.ImageUrls
        };
    }
}

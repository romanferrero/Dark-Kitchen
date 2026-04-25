using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.ProductsControllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService prodService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult CreateProduct(ProductRequestModel request)
    {
        var product = prodService.CreateProduct(ToDto(request));

        return CreatedAtAction(nameof(CreateProduct), null, ToResponse(product));
    }

    [HttpPut("{code}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult UpdateProduct(string code, ProductRequestModel request)
    {
        var product = prodService.UpdateProduct(code, ToDto(request));

        return Ok(ToResponse(product));
    }

    [HttpGet]
    [AuthorizationFilter(UserRole.Client, UserRole.Admin)]
    public IActionResult GetProducts(
    [FromQuery] string? line = null,
    [FromQuery] string? categories = null,
    [FromQuery] string? name = null)
    {
        List<string>? categoryList = null;

        if(!string.IsNullOrWhiteSpace(categories))
        {
            categoryList = [.. categories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim())];
        }

        var products = prodService.GetProducts(line, categoryList, name);

        return Ok(products.Select(ToResponse).ToList());
    }

    private static ProductEntryDto ToDto(ProductRequestModel request)
    {
        return new ProductEntryDto(
            request.Name,
            request.Price,
            request.Description,
            request.Line,
            request.Category,
            request.Images,
            request.Active);
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

using DarkKitchen.Domain.Enums;
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
    public IActionResult CreateProduct(CreateProductRequestModel request)
    {
        var result = prodService.CreateProduct(
            request.Code,
            request.Name,
            request.Description,
            request.Line,
            request.Category,
            request.Images,
            request.Active);

        return CreatedAtAction(nameof(CreateProduct), null, result);
    }

    [HttpPut("{code}")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult UpdateProduct(string code, UpdateProductRequestModel request)
    {
        var result = prodService.UpdateProduct(
            code,
            request.Name,
            request.Description,
            request.Line,
            request.Category,
            request.Images,
            request.Active);

        return Ok(result);
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
            categoryList = categories
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();
        }

        var products = prodService.GetProducts(line, categoryList, name);

        var response = products.Select(p => new ProductResponseModel
        {
            Code = p.Code,
            Name = p.Name,
            Price = p.Price,
            Line = p.Line,
            Category = p.Category,
            ImageUrls = p.Images.Select(i => i.Url).ToList(),
        }).ToList();

        return Ok(response);
    }
}

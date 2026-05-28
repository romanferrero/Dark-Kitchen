using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.ProductsControllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService prodService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(Permission.ManageProducts)]
    public IActionResult CreateProduct(ProductRequestModel request)
    {
        var product = prodService.CreateProduct(ToDto(request));

        return CreatedAtAction(nameof(CreateProduct), null, ToResponse(product));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(Permission.ManageProducts)]
    public IActionResult UpdateProduct(int id, ProductRequestModel request)
    {
        var product = prodService.UpdateProduct(id, ToDto(request));

        return Ok(ToResponse(product));
    }

    [HttpGet]
    [AuthorizationFilter(Permission.ViewProducts)]
    public IActionResult GetProducts([FromQuery] GetProductsQueryModel query)
    {
        List<string>? categoryList = null;

        if(!string.IsNullOrWhiteSpace(query.Categories))
        {
            categoryList = [.. query.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim())];
        }

        var products = prodService.GetProducts(query.Line, categoryList, query.Name);

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

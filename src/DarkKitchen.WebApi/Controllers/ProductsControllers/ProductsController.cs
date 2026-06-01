using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
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
        var product = prodService.CreateProduct(ProductMapper.ToDto(request));

        return CreatedAtAction(nameof(CreateProduct), null, ProductMapper.ToResponse(product));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(Permission.ManageProducts)]
    public IActionResult UpdateProduct(int id, ProductRequestModel request)
    {
        var product = prodService.UpdateProduct(id, ProductMapper.ToDto(request));

        return Ok(ProductMapper.ToResponse(product));
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

        return Ok(products.Select(ProductMapper.ToResponse).ToList());
    }
}

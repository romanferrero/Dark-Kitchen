using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService prodService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateProduct(CreateProductRequestModel request)
    {
        try
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
        catch(ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpPut("{code}")]
    public IActionResult UpdateProduct(string code, UpdateProductRequestModel request)
    {
        try
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
        catch(ArgumentException)
        {
            return BadRequest();
        }
        catch(KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public IActionResult GetProducts(
    [FromQuery] string? line = null,
    [FromQuery] string? categories = null,
    [FromQuery] string? name = null)
    {
        List<string>? categoryList = null;
        if(!string.IsNullOrEmpty(categories))
        {
            categoryList = categories.Split(',').ToList();
        }

        var products = prodService.GetProducts(line, categoryList, name);

        var response = products.Select(p => new ProductResponseModel
        {
            Code = p.Code,
            Name = p.Name,
            Price = p.Price,
            Line = p.Line,
            Category = p.Category,
            ImageUrls = p.ImageUrls,
        }).ToList();

        return Ok(response);
    }
}

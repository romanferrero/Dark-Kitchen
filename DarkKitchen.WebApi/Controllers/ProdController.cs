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
    public IActionResult UpdateProduct(int code, UpdateProductRequestModel request)
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
        catch(KeyNotFoundException)
        {
            return NotFound();
        }
        catch(ArgumentException)
        {
            return BadRequest();
        }
    }
}

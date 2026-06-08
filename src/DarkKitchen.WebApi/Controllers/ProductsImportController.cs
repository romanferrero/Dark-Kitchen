using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsImportController(IProductImportService importService) : ControllerBase
{
    [HttpGet("importers")]
    [AuthorizationFilterAttribute(Permission.ManageProducts)]
    public IActionResult GetImporters()
    {
        var importers = importService.GetAvailableImporters();

        return Ok(importers.Select(ProductImportMapper.ToResponse).ToList());
    }

    [HttpPost("import")]
    [AuthorizationFilterAttribute(Permission.ManageProducts)]
    public IActionResult ImportProducts(ImportRequestModel request)
    {
        var result = importService.ImportProducts(ProductImportMapper.ToDto(request));

        return Ok(ProductImportMapper.ToResponse(result));
    }
}

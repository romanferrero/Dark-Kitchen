using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/admin/reports")]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("top-products")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult GetTopProducts([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
        var result = reportService.GetTopProducts(dateFrom, dateTo);
        return Ok(result);
    }

    [HttpGet("sales")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult GetSalesReport()
    {
        var result = reportService.GetSalesReport();
        return Ok(result);
    }
}

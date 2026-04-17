using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("top-products")]
    [AuthorizationFilter(UserRole.Admin)]
    public IActionResult GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        var result = reportService.GetTopProducts(dateFrom, dateTo);
        return Ok(result);
    }
}

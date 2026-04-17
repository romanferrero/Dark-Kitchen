using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("top-products")]
    public IActionResult GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        var result = reportService.GetTopProducts(dateFrom, dateTo);
        return Ok(result);
    }
}

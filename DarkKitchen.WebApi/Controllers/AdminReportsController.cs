using DarkKitchen.IBusinessLogic;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    public IActionResult GetTopProducts(DateTime dateFrom, DateTime dateTo)
    {
        _ = reportService;
        return Ok();
    }
}

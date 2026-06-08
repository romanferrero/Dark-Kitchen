using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/reports")]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet]
    [AuthorizationFilterAttribute(Permission.ViewReports)]
    public IActionResult GetReport(
        [FromQuery] string type,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        switch(type)
        {
            case "top-products":
                if(!dateFrom.HasValue || !dateTo.HasValue)
                {
                    throw new ArgumentException("Date range (dateFrom and dateTo) is required for top-products.");
                }

                return Ok(reportService.GetTopProducts(dateFrom.Value, dateTo.Value));

            case "sales":
                return Ok(reportService.GetSalesReport());

            default:
                throw new ArgumentException($"Unknown report type '{type}'.");
        }
    }
}

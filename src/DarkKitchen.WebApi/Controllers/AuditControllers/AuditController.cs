using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.AuditModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.AuditControllers;

[ApiController]
[Route("api/audit")]
public class AuditController(IAuditLogService auditService) : ControllerBase
{
    [HttpGet]
    [AuthorizationFilter(Permission.ViewAuditLog)]
    public IActionResult GetAuditLogs([FromQuery] AuditLogQueryModel query)
    {
        if(query.DateFrom == null)
        {
            return BadRequest("DateFrom is required.");
        }

        if(query.DateTo == null)
        {
            return BadRequest("DateTo is required.");
        }

        var logs = auditService.GetAuditLogs(query.DateFrom.Value, query.DateTo.Value, query.EntityName, query.EntityId);

        return Ok(logs);
    }
}

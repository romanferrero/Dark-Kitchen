using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Models.Request.AuditModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController(IAuditLogService auditService) : ControllerBase
{
    [HttpGet]
    [AuthorizationFilterAttribute(Permission.ViewAuditLog)]
    public IActionResult GetAuditLogs([FromQuery] AuditLogQueryModel query)
    {
        var logs = auditService.GetAuditLogs(query.DateFrom, query.DateTo, query.EntityName, query.EntityId);

        return Ok(logs);
    }
}

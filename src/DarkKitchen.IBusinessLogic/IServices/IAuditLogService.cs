using DarkKitchen.IBusinessLogic.DTOs.Exit.AuditLogDTOs;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IAuditLogService
{
    List<AuditLogExitDto> GetAuditLogs(DateTime? dateFrom, DateTime? dateTo, string? entityName = null, int? entityId = null);
}

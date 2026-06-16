using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    public List<AuditLogExitDto> GetAuditLogs(DateTime? dateFrom, DateTime? dateTo, string? entityName = null, int? entityId = null)
    {
        if(!dateFrom.HasValue || !dateTo.HasValue)
        {
            throw new ArgumentException("DateFrom and DateTo are required.");
        }

        if(dateFrom >= dateTo)
        {
            throw new ArgumentException("DateFrom must be earlier than DateTo.");
        }

        return auditLogRepository
            .GetFiltered(dateFrom.Value, dateTo.Value, entityName, entityId)
            .Select(ToExitDto)
            .ToList();
    }

    private static AuditLogExitDto ToExitDto(AuditLog log) =>
        new(log.Id, log.Timestamp, log.EntityName, log.EntityId, log.Description, log.ResponsibleUser);
}

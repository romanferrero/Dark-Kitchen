using DarkKitchen.IBusinessLogic.DTOs.Exit.AuditLogDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
{
    public List<AuditLogExitDto> GetAuditLogs(DateTime dateFrom, DateTime dateTo, string? entityName = null, int? entityId = null)
    {
        if(dateFrom >= dateTo)
        {
            throw new ArgumentException("DateFrom must be earlier than DateTo.");
        }

        return auditLogRepository
            .GetFiltered(dateFrom, dateTo, entityName, entityId)
            .Select(ToExitDto)
            .ToList();
    }

    private static AuditLogExitDto ToExitDto(Domain.Entities.AuditLog log) =>
        new()
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            Description = log.Description,
            ResponsibleUser = log.ResponsibleUser,
        };
}

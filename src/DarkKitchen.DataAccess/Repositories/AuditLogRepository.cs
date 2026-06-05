using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class AuditLogRepository(DbContext context) : Repository<AuditLog>(context), IAuditLogRepository
{
    public List<AuditLog> GetFiltered(DateTime dateFrom, DateTime dateTo, string? entityName = null, int? entityId = null)
    {
        return Context.Set<AuditLog>()
            .Where(a => a.Timestamp >= dateFrom && a.Timestamp < dateTo.AddMinutes(1))
            .Where(a => entityName == null || a.EntityName == entityName)
            .Where(a => entityId == null || a.EntityId == entityId)
            .ToList();
    }
}

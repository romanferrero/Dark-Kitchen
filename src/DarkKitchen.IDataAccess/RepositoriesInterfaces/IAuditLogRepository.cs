using DarkKitchen.Domain.Entities;

namespace DarkKitchen.IDataAccess.RepositoriesInterfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    List<AuditLog> GetFiltered(DateTime dateFrom, DateTime dateTo, string? entityName = null, int? entityId = null);
}

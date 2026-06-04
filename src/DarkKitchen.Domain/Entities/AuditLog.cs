namespace DarkKitchen.Domain.Entities;

public class AuditLog
{
    public int Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string EntityName { get; private set; } = null!;
    public int EntityId { get; private set; }
    public string Description { get; private set; } = null!;
    public string ResponsibleUser { get; private set; } = null!;

    private AuditLog()
    {
    }

    public static AuditLog Create(string entityName, int entityId, string description, string responsibleUser)
    {
        return new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            EntityName = entityName,
            EntityId = entityId,
            Description = description,
            ResponsibleUser = responsibleUser,
        };
    }
}

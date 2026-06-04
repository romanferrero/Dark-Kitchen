namespace DarkKitchen.IBusinessLogic.DTOs.Exit.AuditLogDTOs;

public class AuditLogExitDto
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EntityName { get; set; } = null!;
    public int EntityId { get; set; }
    public string Description { get; set; } = null!;
    public string ResponsibleUser { get; set; } = null!;
}

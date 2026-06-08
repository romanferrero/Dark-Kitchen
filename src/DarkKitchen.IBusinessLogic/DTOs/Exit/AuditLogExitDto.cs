namespace DarkKitchen.IBusinessLogic.DTOs.Exit;

public record AuditLogExitDto(
    int Id,
    DateTime Timestamp,
    string EntityName,
    int EntityId,
    string Description,
    string ResponsibleUser
);

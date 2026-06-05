namespace DarkKitchen.IBusinessLogic.DTOs.Exit.AuditLogDTOs;

public record AuditLogExitDto(
    int Id,
    DateTime Timestamp,
    string EntityName,
    int EntityId,
    string Description,
    string ResponsibleUser
);

namespace DarkKitchen.WebApi.Models.Request.AuditModels;

public class AuditLogQueryModel
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? EntityName { get; set; }
    public int? EntityId { get; set; }
}

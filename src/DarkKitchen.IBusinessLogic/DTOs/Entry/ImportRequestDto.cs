namespace DarkKitchen.IBusinessLogic.DTOs.Entry;

public record ImportRequestDto(string ImporterName, Dictionary<string, string> Parameters);

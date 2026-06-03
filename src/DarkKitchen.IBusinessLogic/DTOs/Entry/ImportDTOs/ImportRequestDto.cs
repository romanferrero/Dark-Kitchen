namespace DarkKitchen.IBusinessLogic.DTOs.Entry.ImportDTOs;

public record ImportRequestDto(string ImporterName, Dictionary<string, string> Parameters);

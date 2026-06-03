namespace DarkKitchen.BusinessLogic.Helpers;

internal static class ProductCodeGenerator
{
    internal static string GenerateUniqueCode(Func<string, bool> exists)
    {
        string code;
        do
        {
            code = $"PROD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
        while(exists(code));

        return code;
    }
}

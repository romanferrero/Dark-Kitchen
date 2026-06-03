using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ImportDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ImportDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using DarkKitchen.Importer;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class ProductImportService(
    IImporterLoader loader,
    IProductRepository productRepository) : IProductImportService
{
    public List<ImporterInfoDto> GetAvailableImporters()
    {
        return loader.LoadImporters()
            .Select(ToInfoDto)
            .ToList();
    }

    public ProductImportResultDto ImportProducts(ImportRequestDto request)
    {
        var importers = loader.LoadImporters();
        var importer = importers.FirstOrDefault(i =>
            i.Name.Equals(request.ImporterName, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException($"Importer '{request.ImporterName}' not found");

        var importedProducts = importer.Import(request.Parameters);
        var result = new ProductImportResultDto();

        foreach(var imported in importedProducts)
        {
            try
            {
                var images = FormatImages(imported.Images);
                var code = GenerateUniqueCode(c => productRepository.Exists(p => p.Code == c));
                var product = Product.Create(code, imported.Name, imported.Price,
                    imported.Description, imported.Line, imported.Category, images, imported.Active);

                productRepository.Add(product);
                result.ImportedCount++;
            }
            catch(ArgumentException ex)
            {
                result.Errors.Add($"Producto '{imported.Name}': {ex.Message}");
            }
        }

        return result;
    }

    private static string FormatImages(IReadOnlyCollection<ImportedProductImage> images)
    {
        return string.Join(",", images.Select(img => $"{img.Path}|{img.SizeInKb}"));
    }

    private static string GenerateUniqueCode(Func<string, bool> exists)
    {
        string code;
        do
        {
            code = $"PROD-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
        while(exists(code));

        return code;
    }

    private static ImporterInfoDto ToInfoDto(IProductImporter importer)
    {
        return new ImporterInfoDto
        {
            Name = importer.Name,
            Description = importer.Description,
            Parameters = importer.Parameters.Select(p => new ImporterParameterDto
            {
                Name = p.Name,
                Label = p.Label,
                Description = p.Description,
                Required = p.Required
            }).ToList()
        };
    }
}

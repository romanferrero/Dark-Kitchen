using DarkKitchen.BusinessLogic.Helpers;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using DarkKitchen.Importer;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class ProductImportService(
    IImporterLoader loader,
    IProductRepository productRepository,
    IImageFileReader imageFileReader) : IProductImportService
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
                var code = ProductCodeGenerator.GenerateUniqueCode(c => productRepository.Exists(p => p.Code == c));
                var product = Product.Create(new CreateProductParamsDto(code, imported.Name, imported.Price,
                    imported.Description, imported.Line, imported.Category, images, imported.Active));

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

    private string FormatImages(IReadOnlyCollection<ImportedProductImage> images)
    {
        return string.Join('\n', images.Select(img => imageFileReader.ReadAsDataUri(img.Path)));
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

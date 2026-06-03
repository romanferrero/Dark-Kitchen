using DarkKitchen.IBusinessLogic.DTOs.Entry.ImportDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ImportDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.ProductsControllers;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.ProductsControllersTests;

[TestClass]
public class ProductsImportControllerTests
{
    private Mock<IProductImportService> _importServiceMock = null!;
    private ProductsImportController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _importServiceMock = new Mock<IProductImportService>(MockBehavior.Strict);
        _controller = new ProductsImportController(_importServiceMock.Object);
    }

    [TestMethod]
    public void GetImporters_ReturnsOkWithImporterList()
    {
        var importers = new List<ImporterInfoDto>
        {
            new()
            {
                Name = "JSON",
                Description = "Importa productos desde archivo JSON",
                Parameters =
                [
                    new ImporterParameterDto
                    {
                        Name = "filePath",
                        Label = "Ruta del archivo",
                        Description = "Ruta al archivo JSON",
                        Required = true
                    }
                ]
            }
        };

        _importServiceMock
            .Setup(s => s.GetAvailableImporters())
            .Returns(importers);

        var result = _controller.GetImporters() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as List<ImporterInfoResponseModel>;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Count);
        Assert.AreEqual("JSON", response[0].Name);
        Assert.AreEqual(1, response[0].Parameters.Count);
        Assert.AreEqual("filePath", response[0].Parameters[0].Name);
    }

    [TestMethod]
    public void ImportProducts_ValidRequest_ReturnsOkWithResult()
    {
        var request = new ImportRequestModel
        {
            ImporterName = "JSON",
            Parameters = new Dictionary<string, string> { { "filePath", "/data/productos.json" } }
        };

        _importServiceMock
            .Setup(s => s.ImportProducts(It.IsAny<ImportRequestDto>()))
            .Returns(new ProductImportResultDto { ImportedCount = 3, Errors = [] });

        var result = _controller.ImportProducts(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as ProductImportResultResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(3, response.ImportedCount);
        Assert.AreEqual(0, response.Errors.Count);
    }

    [TestMethod]
    public void ImportProducts_ImporterNotFound_ThrowsKeyNotFoundException()
    {
        var request = new ImportRequestModel
        {
            ImporterName = "Inexistente",
            Parameters = new Dictionary<string, string>()
        };

        _importServiceMock
            .Setup(s => s.ImportProducts(It.IsAny<ImportRequestDto>()))
            .Throws(new KeyNotFoundException("Importer 'Inexistente' not found"));

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _controller.ImportProducts(request));
    }

    [TestMethod]
    public void ImportProducts_WithErrors_ReturnsOkWithPartialResult()
    {
        var request = new ImportRequestModel
        {
            ImporterName = "JSON",
            Parameters = new Dictionary<string, string> { { "filePath", "/data/productos.json" } }
        };

        _importServiceMock
            .Setup(s => s.ImportProducts(It.IsAny<ImportRequestDto>()))
            .Returns(new ProductImportResultDto
            {
                ImportedCount = 2,
                Errors = ["Producto 'AB': Product name must be between 10 and 50 characters."]
            });

        var result = _controller.ImportProducts(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as ProductImportResultResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.ImportedCount);
        Assert.AreEqual(1, response.Errors.Count);
    }
}

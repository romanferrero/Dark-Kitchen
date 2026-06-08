using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using DarkKitchen.Importer;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class ProductImportServiceTests
{
    private Mock<IImporterLoader> _loaderMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private ProductImportService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _loaderMock = new Mock<IImporterLoader>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _service = new ProductImportService(_loaderMock.Object, _productRepoMock.Object);
    }

    private static Mock<IProductImporter> CreateMockImporter(
        string name = "JSON",
        string description = "Importa desde JSON",
        List<ImporterParameter>? parameters = null)
    {
        var mock = new Mock<IProductImporter>(MockBehavior.Strict);
        mock.Setup(i => i.Name).Returns(name);
        mock.Setup(i => i.Description).Returns(description);
        mock.Setup(i => i.Parameters).Returns(parameters ??
        [
            new ImporterParameter("filePath", "Ruta del archivo", "Ruta al archivo", true)
        ]);
        return mock;
    }

    private static ImportedProduct CreateImportedProduct(
        string name = "Hamburguesa clasica especial",
        decimal price = 150m,
        string description = "Hamburguesa con lechuga y tomate fresco",
        string line = "Combo burgers",
        string category = "Parrilla",
        string imageUrl = "http://img.com/burg.jpg",
        decimal imageSize = 100m,
        bool active = true)
    {
        return new ImportedProduct(name, price, description, line, category,
            [new ImportedProductImage(imageUrl, imageSize)], active);
    }

    [TestMethod]
    public void GetAvailableImporters_MapsLoadedImportersToDto()
    {
        var importerMock = CreateMockImporter();

        _loaderMock
            .Setup(l => l.LoadImporters())
            .Returns([importerMock.Object]);

        var result = _service.GetAvailableImporters();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("JSON", result[0].Name);
        Assert.AreEqual("Importa desde JSON", result[0].Description);
        Assert.AreEqual(1, result[0].Parameters.Count);
        Assert.AreEqual("filePath", result[0].Parameters[0].Name);
    }

    [TestMethod]
    public void GetAvailableImporters_NoImporters_ReturnsEmptyList()
    {
        _loaderMock
            .Setup(l => l.LoadImporters())
            .Returns([]);

        var result = _service.GetAvailableImporters();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void ImportProducts_ValidProducts_PersistsAllAndReturnsCount()
    {
        var importerMock = CreateMockImporter();
        var products = new List<ImportedProduct>
        {
            CreateImportedProduct(name: "Hamburguesa clasica especial"),
            CreateImportedProduct(name: "Ravioles rellenos de ricota")
        };

        importerMock
            .Setup(i => i.Import(It.IsAny<IReadOnlyDictionary<string, string>>()))
            .Returns(products);

        _loaderMock
            .Setup(l => l.LoadImporters())
            .Returns([importerMock.Object]);

        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        var request = new ImportRequestDto("JSON", new Dictionary<string, string>
        {
            { "filePath", "/data/productos.json" }
        });

        var result = _service.ImportProducts(request);

        Assert.AreEqual(2, result.ImportedCount);
        Assert.AreEqual(0, result.Errors.Count);
        _productRepoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Exactly(2));
    }

    [TestMethod]
    public void ImportProducts_ImporterNotFound_ThrowsKeyNotFoundException()
    {
        _loaderMock
            .Setup(l => l.LoadImporters())
            .Returns([]);

        var request = new ImportRequestDto("Inexistente", []);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _service.ImportProducts(request));
    }

    [TestMethod]
    public void ImportProducts_InvalidProduct_AccumulatesErrorAndContinues()
    {
        var importerMock = CreateMockImporter();
        var products = new List<ImportedProduct>
        {
            CreateImportedProduct(name: "AB"),
            CreateImportedProduct(name: "Hamburguesa clasica especial")
        };

        importerMock
            .Setup(i => i.Import(It.IsAny<IReadOnlyDictionary<string, string>>()))
            .Returns(products);

        _loaderMock
            .Setup(l => l.LoadImporters())
            .Returns([importerMock.Object]);

        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        var request = new ImportRequestDto("JSON", new Dictionary<string, string>
        {
            { "filePath", "/data/productos.json" }
        });

        var result = _service.ImportProducts(request);

        Assert.AreEqual(1, result.ImportedCount);
        Assert.AreEqual(1, result.Errors.Count);
        StringAssert.Contains(result.Errors[0], "AB");
        _productRepoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void ImportProducts_MatchesImporterNameCaseInsensitive()
    {
        var importerMock = CreateMockImporter(name: "JSON");
        var products = new List<ImportedProduct>
        {
            CreateImportedProduct()
        };

        importerMock
            .Setup(i => i.Import(It.IsAny<IReadOnlyDictionary<string, string>>()))
            .Returns(products);

        _loaderMock
            .Setup(l => l.LoadImporters())
            .Returns([importerMock.Object]);

        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        var request = new ImportRequestDto("json", new Dictionary<string, string>
        {
            { "filePath", "/data/productos.json" }
        });

        var result = _service.ImportProducts(request);

        Assert.AreEqual(1, result.ImportedCount);
    }
}

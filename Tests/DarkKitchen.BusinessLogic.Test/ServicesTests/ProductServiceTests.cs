using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class ProductServiceTests
{
    private Mock<IProductRepository> _productRepoMock = null!;
    private ProductService _productService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _productService = new ProductService(_productRepoMock.Object);
    }

    private static Product CreateProduct(
        string code = "PROD-001",
        string name = "Producto valido de testing",
        decimal price = 100m,
        string description = "Descripcion valida suficientemente larga para dominio",
        string line = "LineA",
        string category = "CategoryA",
        string images = "http://img.com/test.jpg|100",
        bool active = true)
    {
        return Product.Create(code, name, price, description, line, category, images, active);
    }

    [TestMethod]
    public void GetProducts_WithLineFilter_DelegatesToRepository()
    {
        var storedProducts = new List<Product>
        {
            CreateProduct(code: "BURG01", line: "Combo burgers")
        };

        _productRepoMock
            .Setup(r => r.GetFiltered("Combo burgers", null, null))
            .Returns(storedProducts);

        var result = _productService.GetProducts("Combo burgers", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);

        _productRepoMock.Verify(r => r.GetFiltered("Combo burgers", null, null), Times.Once);
    }

    [TestMethod]
    public void GetProducts_WithCategoryFilter_DelegatesToRepository()
    {
        var storedProducts = new List<Product>
        {
            CreateProduct(code: "PAST01", category: "Pastas")
        };

        var categories = new List<string> { "Pastas" };

        _productRepoMock
            .Setup(r => r.GetFiltered(null, categories, null))
            .Returns(storedProducts);

        var result = _productService.GetProducts(null, categories, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pastas", result[0].Category);

        _productRepoMock.Verify(r => r.GetFiltered(null, categories, null), Times.Once);
    }

    [TestMethod]
    public void GetProducts_WithNameFilter_DelegatesToRepository()
    {
        var storedProducts = new List<Product>
        {
            CreateProduct(code: "BURG01", name: "Hamburguesa clasica especial")
        };

        _productRepoMock
            .Setup(r => r.GetFiltered(null, null, "Hamburguesa"))
            .Returns(storedProducts);

        var result = _productService.GetProducts(null, null, "Hamburguesa");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Hamburguesa clasica especial", result[0].Name);

        _productRepoMock.Verify(r => r.GetFiltered(null, null, "Hamburguesa"), Times.Once);
    }

    [TestMethod]
    public void GetProducts_WithAllFilters_DelegatesToRepository()
    {
        var storedProducts = new List<Product>
        {
            CreateProduct(code: "BURG01", line: "Combo burgers", category: "Parrilla")
        };

        var categories = new List<string> { "Parrilla" };

        _productRepoMock
            .Setup(r => r.GetFiltered("Combo burgers", categories, "Hamburguesa"))
            .Returns(storedProducts);

        var result = _productService.GetProducts("Combo burgers", categories, "Hamburguesa");

        Assert.AreEqual(1, result.Count);

        _productRepoMock.Verify(
            r => r.GetFiltered("Combo burgers", categories, "Hamburguesa"),
            Times.Once);
    }

    [TestMethod]
    public void GetProducts_NoFilters_ReturnsAllProducts()
    {
        var storedProducts = new List<Product>
        {
            CreateProduct(code: "BURG01"),
            CreateProduct(code: "PAST01")
        };

        _productRepoMock
            .Setup(r => r.GetFiltered(null, null, null))
            .Returns(storedProducts);

        var result = _productService.GetProducts(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetProducts_NoMatches_ReturnsEmptyList()
    {
        _productRepoMock
            .Setup(r => r.GetFiltered("Inexistente", null, null))
            .Returns([]);

        var result = _productService.GetProducts("Inexistente", null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetProducts_FiltersOutInactiveProducts()
    {
        var storedProducts = new List<Product>
        {
            CreateProduct(code: "BURG01", active: true),
            CreateProduct(code: "BURG02", active: false)
        };

        _productRepoMock
            .Setup(r => r.GetFiltered(null, null, null))
            .Returns(storedProducts);

        var result = _productService.GetProducts(null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void CreateProduct_ValidData_CallsRepositoryAdd()
    {
        var dto = new ProductEntryDto(
            "Hamburguesa clasica especial",
            100m,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg1.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns([]);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        _productService.CreateProduct(dto);

        _productRepoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ValidData_CallsRepositoryUpdate()
    {
        var existing = CreateProduct(code: "BURG01");

        var dto = new ProductEntryDto(
            "Hamburguesa especial actualizada",
            150m,
            "Hamburguesa con doble carne",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg2.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns([existing]);

        _productRepoMock
            .Setup(r => r.Update(It.IsAny<Product>()));

        _productService.UpdateProduct("BURG01", dto);

        _productRepoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new ProductEntryDto(
            "Hamburguesa especial",
            150m,
            "Hamburguesa con doble carne",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg2.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns([]);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _productService.UpdateProduct("NOEXISTE", dto));
    }
}

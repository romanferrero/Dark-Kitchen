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

    private void SetupGetFiltered(List<Product> storedProducts)
    {
        _productRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Product, bool>>?>()))
            .Returns((Expression<Func<Product, bool>>? predicate) =>
                predicate == null
                    ? storedProducts
                    : storedProducts.Where(predicate.Compile()).ToList());
    }

    [TestMethod]
    public void GetProducts_WithLineFilter_DelegatesToRepository()
    {
        SetupGetFiltered(
        [
            CreateProduct(code: "BURG01", line: "Combo burgers"),
            CreateProduct(code: "PAST01", line: "Pastas")
        ]);

        var result = _productService.GetProducts("Combo burgers", null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void GetProducts_WithCategoryFilter_DelegatesToRepository()
    {
        SetupGetFiltered(
        [
            CreateProduct(code: "PAST01", category: "Pastas"),
            CreateProduct(code: "BURG01", category: "Parrilla")
        ]);

        var result = _productService.GetProducts(null, ["Pastas"], null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pastas", result[0].Category);
    }

    [TestMethod]
    public void GetProducts_WithNameFilter_DelegatesToRepository()
    {
        SetupGetFiltered(
        [
            CreateProduct(code: "BURG01", name: "Hamburguesa clasica especial"),
            CreateProduct(code: "PAST01", name: "Ravioles clasicos rellenos")
        ]);

        var result = _productService.GetProducts(null, null, "Hamburguesa");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Hamburguesa clasica especial", result[0].Name);
    }

    [TestMethod]
    public void GetProducts_WithAllFilters_DelegatesToRepository()
    {
        SetupGetFiltered(
        [
            CreateProduct(
                code: "BURG01",
                name: "Hamburguesa clasica especial",
                line: "Combo burgers",
                category: "Parrilla")
        ]);

        var result = _productService.GetProducts("Combo burgers", ["Parrilla"], "Hamburguesa");

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetProducts_NoFilters_ReturnsAllProducts()
    {
        SetupGetFiltered(
        [
            CreateProduct(code: "BURG01"),
            CreateProduct(code: "PAST01")
        ]);

        var result = _productService.GetProducts(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetProducts_NoMatches_ReturnsEmptyList()
    {
        SetupGetFiltered([]);

        var result = _productService.GetProducts("Inexistente", null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetProducts_FiltersOutInactiveProducts()
    {
        SetupGetFiltered(
        [
            CreateProduct(code: "BURG01", active: true),
            CreateProduct(code: "BURG02", active: false)
        ]);

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
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        var result = _productService.CreateProduct(dto);

        _productRepoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        Assert.AreEqual("Hamburguesa con lechuga y tomate fresco", result.Description);
        Assert.IsTrue(result.Active);
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
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(existing);

        _productRepoMock
            .Setup(r => r.Update(It.IsAny<Product>()));

        _productService.UpdateProduct(1, dto);

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
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _productService.UpdateProduct(99999, dto));
    }
}

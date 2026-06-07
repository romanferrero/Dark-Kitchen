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
    private Mock<IAuditLogRepository> _auditRepoMock = null!;
    private ProductService _productService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _auditRepoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        _productService = new ProductService(_productRepoMock.Object, _auditRepoMock.Object);
    }

    private static Product CreateProduct(
        string code = "PROD-001",
        string name = "Valid product for testing",
        decimal price = 100m,
        string description = "Valid description long enough for domain",
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
            CreateProduct(code: "BURG01", name: "Classic special burger"),
            CreateProduct(code: "PAST01", name: "Classic stuffed ravioli")
        ]);

        var result = _productService.GetProducts(null, null, "Burger");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Classic special burger", result[0].Name);
    }

    [TestMethod]
    public void GetProducts_WithAllFilters_DelegatesToRepository()
    {
        SetupGetFiltered(
        [
            CreateProduct(
                code: "BURG01",
                name: "Classic special burger",
                line: "Combo burgers",
                category: "Parrilla")
        ]);

        var result = _productService.GetProducts("Combo burgers", ["Parrilla"], "Burger");

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

        var result = _productService.GetProducts("Nonexistent", null, null);

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
            "Classic special burger",
            100m,
            "Burger with lettuce and fresh tomato",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg1.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        _auditRepoMock
            .Setup(r => r.Add(It.IsAny<AuditLog>()));

        var result = _productService.CreateProduct(dto, "admin@darkkitchen.com");

        _productRepoMock.Verify(r => r.Add(It.IsAny<Product>()), Times.Once);
        Assert.AreEqual("Burger with lettuce and fresh tomato", result.Description);
        Assert.IsTrue(result.Active);
    }

    [TestMethod]
    public void CreateProduct_ValidData_AddsAuditLogWithCorrectData()
    {
        var dto = new ProductEntryDto(
            "Classic special burger",
            100m,
            "Burger with lettuce and fresh tomato",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg1.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        _productRepoMock
            .Setup(r => r.Add(It.IsAny<Product>()));

        _auditRepoMock
            .Setup(r => r.Add(It.IsAny<AuditLog>()));

        _productService.CreateProduct(dto, "admin@darkkitchen.com");

        _auditRepoMock.Verify(
            r => r.Add(It.Is<AuditLog>(a =>
                a.EntityName == "PRODUCT" &&
                a.Description == "Creation" &&
                a.ResponsibleUser == "admin@darkkitchen.com")),
            Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ValidData_CallsRepositoryUpdate()
    {
        var existing = CreateProduct(code: "BURG01");

        var dto = new ProductEntryDto(
            "Updated special burger",
            150m,
            "Burger with double patty",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg2.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(existing);

        _productRepoMock
            .Setup(r => r.Update(It.IsAny<Product>()));

        _auditRepoMock
            .Setup(r => r.Add(It.IsAny<AuditLog>()));

        _productService.UpdateProduct(1, dto, "admin@darkkitchen.com");

        _productRepoMock.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
    }

    [TestMethod]
    public void UpdateProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var dto = new ProductEntryDto(
            "Hamburguesa especial",
            150m,
            "Burger with double patty",
            "Combo burgers",
            "Parrilla",
            "http://img.com/burg2.jpg|100",
            true);

        _productRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _productService.UpdateProduct(99999, dto, "admin@darkkitchen.com"));
    }
}

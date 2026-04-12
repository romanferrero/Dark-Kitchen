using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class ProductsControllerTests
{
    private Mock<IProductService> _prodServiceMock = null!;
    private ProductsController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _prodServiceMock = new Mock<IProductService>(MockBehavior.Strict);
        _controller = new ProductsController(_prodServiceMock.Object);
    }

    private static Product MakeProduct(string code, string name)
    {
        return Product.Create(
            code: code,
            name: name,
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg|100",
            active: true);
    }

    [TestMethod]
    public void CreateProduct_ValidData_Returns201()
    {
        var request = new CreateProductRequestModel
        {
            Code = "PAP01",
            Name = "papas fritas",
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Returns("Creado con exito");

        var result = _controller.CreateProduct(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual("Creado con exito", result.Value);
    }

    [TestMethod]
    public void CreateProduct_InvalidData_Returns400()
    {
        var request = new CreateProductRequestModel
        {
            Code = "PAP01",
            Name = string.Empty,
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws(new ArgumentException("Datos invalidos"));

        var result = _controller.CreateProduct(request) as BadRequestResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_ValidData_Returns200()
    {
        var request = new UpdateProductRequestModel
        {
            Name = "papas medianas",
            Description = "menos crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "nuevas-imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Returns("Actualizado con exito");

        var result = _controller.UpdateProduct("PAP01", request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("Actualizado con exito", result.Value);
    }

    [TestMethod]
    public void UpdateProduct_InvalidData_Returns400()
    {
        var request = new UpdateProductRequestModel
        {
            Name = string.Empty,
            Description = "menos crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "nuevas-imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws(new ArgumentException("El nombre no puede estar vacío"));

        var result = _controller.UpdateProduct("PAP01", request) as BadRequestResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_ProductNotFound_Returns404()
    {
        var request = new UpdateProductRequestModel
        {
            Name = "papas medianas",
            Description = "menos crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "nuevas-imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws(new KeyNotFoundException());

        var result = _controller.UpdateProduct("UNKNOWN", request) as NotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public void GetProducts_WithFilters_ReturnsOkWithList()
    {
        var expectedProducts = new List<Product> { MakeProduct("BURG01", "Hamburguesa clasica") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                It.IsAny<string?>(),
                It.IsAny<List<string>?>(),
                It.IsAny<string?>()))
            .Returns(expectedProducts);

        var result = _controller.GetProducts("Combo burgers", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(1, products.Count);
        Assert.AreEqual("BURG01", products[0].Code);
    }

    [TestMethod]
    public void GetProducts_NoFilters_ReturnsOkWithAllProducts()
    {
        var expectedProducts = new List<Product>
        {
            MakeProduct("BURG01", "Hamburguesa clasica"),
            MakeProduct("PAST01", "Ravioles clasicos")
        };

        _prodServiceMock
            .Setup(s => s.GetProducts(null, null, null))
            .Returns(expectedProducts);

        var result = _controller.GetProducts(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(2, products.Count);
    }

    [TestMethod]
    public void GetProducts_NoResults_ReturnsOkWithEmptyList()
    {
        _prodServiceMock
            .Setup(s => s.GetProducts(
                It.IsAny<string?>(),
                It.IsAny<List<string>?>(),
                It.IsAny<string?>()))
            .Returns([]);

        var result = _controller.GetProducts("Inexistente", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(0, products.Count);
    }

    [TestMethod]
    public void GetProducts_WithCategoriesQuery_ParsesCategoriesAndReturnsOk()
    {
        var expectedProducts = new List<Product> { MakeProduct("BURG01", "Hamburguesa clasica") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                "Combo burgers",
                It.Is<List<string>>(c =>
                    c.Count == 2 &&
                    c[0] == "Parrilla" &&
                    c[1] == "Pastas"),
                "Hamburguesa"))
            .Returns(expectedProducts);

        var result = _controller.GetProducts(
            "Combo burgers",
            "Parrilla, Pastas",
            "Hamburguesa") as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(1, products.Count);
        Assert.AreEqual("BURG01", products[0].Code);
    }
}

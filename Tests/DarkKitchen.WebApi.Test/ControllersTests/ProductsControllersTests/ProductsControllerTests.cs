using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.ProductsControllers;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.ProductsControllersTests;

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

    private static ProductExitDTO MakeProductDTO(string code, string name)
    {
        return new ProductExitDTO
        {
            Code = code,
            Name = name,
            Price = 100m,
            Line = "Combo burgers",
            Category = "Parrilla",
            ImageUrls = ["http://img.com/burg1.jpg"]
        };
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
            .Returns(MakeProductDTO("PAP01", "papas fritas"));

        var result = _controller.CreateProduct(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        var response = result.Value as ProductResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual("PAP01", response.Code);
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

        Assert.ThrowsException<ArgumentException>(() => _controller.CreateProduct(request));
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
            .Returns(MakeProductDTO("PAP01", "papas medianas"));

        var result = _controller.UpdateProduct("PAP01", request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var response = result.Value as ProductResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual("PAP01", response.Code);
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

        Assert.ThrowsException<ArgumentException>(() => _controller.UpdateProduct("PAP01", request));
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

        Assert.ThrowsException<KeyNotFoundException>(() => _controller.UpdateProduct("UNKNOWN", request));
    }

    [TestMethod]
    public void GetProducts_WithFilters_ReturnsOkWithList()
    {
        var expected = new List<ProductExitDTO> { MakeProductDTO("BURG01", "Hamburguesa clasica") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                It.IsAny<string?>(),
                It.IsAny<List<string>?>(),
                It.IsAny<string?>()))
            .Returns(expected);

        var result = _controller.GetProducts("Combo burgers") as OkObjectResult;

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
        var expected = new List<ProductExitDTO>
        {
            MakeProductDTO("BURG01", "Hamburguesa clasica"),
            MakeProductDTO("PAST01", "Ravioles clasicos")
        };

        _prodServiceMock
            .Setup(s => s.GetProducts(null, null, null))
            .Returns(expected);

        var result = _controller.GetProducts() as OkObjectResult;

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

        var result = _controller.GetProducts("Inexistente") as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(0, products.Count);
    }

    [TestMethod]
    public void GetProducts_WithCategoriesQuery_ParsesCategoriesAndReturnsOk()
    {
        var expected = new List<ProductExitDTO> { MakeProductDTO("BURG01", "Hamburguesa clasica") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                "Combo burgers",
                It.Is<List<string>>(c =>
                    c.Count == 2 &&
                    c[0] == "Parrilla" &&
                    c[1] == "Pastas"),
                "Hamburguesa"))
            .Returns(expected);

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

using DarkKitchen.IBusinessLogic.DTOs.Entry.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.ProductsControllers;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.ProductsModels;
using DarkKitchen.WebApi.Models.Response.ProductsModels;
using Microsoft.AspNetCore.Http;
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
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.HttpContext.Items["UserId"] = 1;
    }

    private static ProductExitDto MakeProductDTO(string code, string name)
    {
        return new ProductExitDto
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
        var request = new ProductRequestModel
        {
            Name = "papas fritas",
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "img1",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(It.IsAny<ProductEntryDto>(), It.IsAny<string>()))
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
        var request = new ProductRequestModel
        {
            Name = string.Empty,
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(It.IsAny<ProductEntryDto>(), It.IsAny<string>()))
            .Throws(new ArgumentException("Datos invalidos"));

        Assert.ThrowsException<ArgumentException>(() => _controller.CreateProduct(request));
    }

    [TestMethod]
    public void UpdateProduct_ValidData_Returns200()
    {
        var request = new ProductRequestModel
        {
            Name = "papas medianas",
            Description = "menos crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "img2",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<int>(),
                It.IsAny<ProductEntryDto>(),
                It.IsAny<string>()))
            .Returns(MakeProductDTO("PAP01", "papas medianas"));

        var result = _controller.UpdateProduct(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as ProductResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual("PAP01", response.Code);
    }

    [TestMethod]
    public void UpdateProduct_InvalidData_Returns400()
    {
        var request = new ProductRequestModel
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
                It.IsAny<int>(),
                It.IsAny<ProductEntryDto>(),
                It.IsAny<string>()))
            .Throws(new ArgumentException("El nombre no puede estar vacío"));

        Assert.ThrowsException<ArgumentException>(() => _controller.UpdateProduct(1, request));
    }

    [TestMethod]
    public void UpdateProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var request = new ProductRequestModel
        {
            Name = "papas medianas",
            Description = "menos crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "img2",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<int>(),
                It.IsAny<ProductEntryDto>(),
                It.IsAny<string>()))
            .Throws(new KeyNotFoundException());

        Assert.ThrowsException<KeyNotFoundException>(() => _controller.UpdateProduct(99999, request));
    }

    [TestMethod]
    public void GetProducts_WithFilters_ReturnsOkWithList()
    {
        var expected = new List<ProductExitDto> { MakeProductDTO("BURG01", "Hamburguesa clasica") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                It.IsAny<string?>(),
                It.IsAny<List<string>?>(),
                It.IsAny<string?>()))
            .Returns(expected);

        var result = _controller.GetProducts(new GetProductsQueryModel { Line = "Combo burgers" }) as OkObjectResult;

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
        var expected = new List<ProductExitDto>
        {
            MakeProductDTO("BURG01", "Hamburguesa clasica"),
            MakeProductDTO("PAST01", "Ravioles clasicos")
        };

        _prodServiceMock
            .Setup(s => s.GetProducts(null, null, null))
            .Returns(expected);

        var result = _controller.GetProducts(new GetProductsQueryModel()) as OkObjectResult;

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

        var result = _controller.GetProducts(new GetProductsQueryModel { Line = "Inexistente" }) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(0, products.Count);
    }

    [TestMethod]
    public void GetProducts_WithCategoriesQuery_ParsesCategoriesAndReturnsOk()
    {
        var expected = new List<ProductExitDto> { MakeProductDTO("BURG01", "Hamburguesa clasica") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                "Combo burgers",
                It.Is<List<string>>(c =>
                    c.Count == 2 &&
                    c[0] == "Parrilla" &&
                    c[1] == "Pastas"),
                "Hamburguesa"))
            .Returns(expected);

        var result = _controller.GetProducts(new GetProductsQueryModel
        {
            Line = "Combo burgers",
            Categories = "Parrilla, Pastas",
            Name = "Hamburguesa"
        }) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = result.Value as List<ProductResponseModel>;
        Assert.IsNotNull(products);
        Assert.AreEqual(1, products.Count);
        Assert.AreEqual("BURG01", products[0].Code);
    }
}

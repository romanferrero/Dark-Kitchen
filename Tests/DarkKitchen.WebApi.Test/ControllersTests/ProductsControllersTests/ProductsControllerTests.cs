using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers;
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
            Description = "Burger with lettuce and fresh tomato",
            Line = "Combo burgers",
            Category = "Grill",
            Active = true,
            ImageUrls = ["http://img.com/burg1.jpg"]
        };
    }

    [TestMethod]
    public void CreateProduct_ValidData_Returns201()
    {
        var request = new ProductRequestModel
        {
            Name = "french fries",
            Description = "crispy",
            Line = "snacks",
            Category = "fried foods",
            Images = "img1",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(It.IsAny<ProductEntryDto>(), It.IsAny<string>()))
            .Returns(MakeProductDTO("PAP01", "french fries"));

        var result = _controller.CreateProduct(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);

        var response = result.Value as ProductResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual("PAP01", response.Code);
        Assert.AreEqual("Burger with lettuce and fresh tomato", response.Description);
        Assert.IsTrue(response.Active);
    }

    [TestMethod]
    public void CreateProduct_InvalidData_Returns400()
    {
        var request = new ProductRequestModel
        {
            Name = string.Empty,
            Description = "crispy",
            Line = "snacks",
            Category = "fried foods",
            Images = "images",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(It.IsAny<ProductEntryDto>(), It.IsAny<string>()))
            .Throws(new ArgumentException("Invalid data"));

        Assert.ThrowsException<ArgumentException>(() => _controller.CreateProduct(request));
    }

    [TestMethod]
    public void UpdateProduct_ValidData_Returns200()
    {
        var request = new ProductRequestModel
        {
            Name = "medium fries",
            Description = "less crispy",
            Line = "snacks",
            Category = "fried foods",
            Images = "img2",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<int>(),
                It.IsAny<ProductEntryDto>(),
                It.IsAny<string>()))
            .Returns(MakeProductDTO("PAP01", "medium fries"));

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
            Description = "less crispy",
            Line = "snacks",
            Category = "fried foods",
            Images = "new-images",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<int>(),
                It.IsAny<ProductEntryDto>(),
                It.IsAny<string>()))
            .Throws(new ArgumentException("Name cannot be empty"));

        Assert.ThrowsException<ArgumentException>(() => _controller.UpdateProduct(1, request));
    }

    [TestMethod]
    public void UpdateProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var request = new ProductRequestModel
        {
            Name = "medium fries",
            Description = "less crispy",
            Line = "snacks",
            Category = "fried foods",
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
        var expected = new List<ProductExitDto> { MakeProductDTO("BURG01", "Classic burger") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                It.IsAny<string?>(),
                It.IsAny<List<string>?>(),
                It.IsAny<string?>()))
            .Returns(expected);

        var result = _controller.GetProducts(new GetProductsQueryModel { Line = "Combo burgers" }) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = (result.Value as PagedResultResponseModel<ProductResponseModel>)?.Items;
        Assert.IsNotNull(products);
        Assert.AreEqual(1, products.Count);
        Assert.AreEqual("BURG01", products[0].Code);
    }

    [TestMethod]
    public void GetProducts_NoFilters_ReturnsOkWithAllProducts()
    {
        var expected = new List<ProductExitDto>
        {
            MakeProductDTO("BURG01", "Classic burger"),
            MakeProductDTO("PAST01", "Classic ravioli")
        };

        _prodServiceMock
            .Setup(s => s.GetProducts(null, null, null))
            .Returns(expected);

        var result = _controller.GetProducts(new GetProductsQueryModel()) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = (result.Value as PagedResultResponseModel<ProductResponseModel>)?.Items;
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

        var result = _controller.GetProducts(new GetProductsQueryModel { Line = "Nonexistent" }) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = (result.Value as PagedResultResponseModel<ProductResponseModel>)?.Items;
        Assert.IsNotNull(products);
        Assert.AreEqual(0, products.Count);
    }

    [TestMethod]
    public void CreateProduct_NullUserId_PassesEmptyStringToService()
    {
        _controller.HttpContext.Items["UserId"] = null;

        var request = new ProductRequestModel
        {
            Name = "french fries",
            Description = "crispy",
            Line = "snacks",
            Category = "fried foods",
            Images = "img1",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(It.IsAny<ProductEntryDto>(), string.Empty))
            .Returns(MakeProductDTO("PAP01", "french fries"));

        var result = _controller.CreateProduct(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
    }

    [TestMethod]
    public void UpdateProduct_NullUserId_PassesEmptyStringToService()
    {
        _controller.HttpContext.Items["UserId"] = null;

        var request = new ProductRequestModel
        {
            Name = "medium fries",
            Description = "less crispy",
            Line = "snacks",
            Category = "fried foods",
            Images = "img2",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(It.IsAny<int>(), It.IsAny<ProductEntryDto>(), string.Empty))
            .Returns(MakeProductDTO("PAP01", "medium fries"));

        var result = _controller.UpdateProduct(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetProducts_WithCategoriesQuery_ParsesCategoriesAndReturnsOk()
    {
        var expected = new List<ProductExitDto> { MakeProductDTO("BURG01", "Classic burger") };

        _prodServiceMock
            .Setup(s => s.GetProducts(
                "Combo burgers",
                It.Is<List<string>>(c =>
                    c.Count == 2 &&
                    c[0] == "Parrilla" &&
                    c[1] == "Pastas"),
                "Burger"))
            .Returns(expected);

        var result = _controller.GetProducts(new GetProductsQueryModel
        {
            Line = "Combo burgers",
            Categories = "Parrilla, Pastas",
            Name = "Burger"
        }) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var products = (result.Value as PagedResultResponseModel<ProductResponseModel>)?.Items;
        Assert.IsNotNull(products);
        Assert.AreEqual(1, products.Count);
        Assert.AreEqual("BURG01", products[0].Code);
    }
}

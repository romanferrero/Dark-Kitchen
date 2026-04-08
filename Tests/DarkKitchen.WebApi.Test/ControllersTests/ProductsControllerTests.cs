using DarkKitchen.BusinessLogic.Interfaces;
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

    [TestMethod]
    public void CreateProduct_ValidData_Returns201()
    {
        var request = new CreateProductRequestModel
        {
            Code = 1,
            Name = "papas fritas",
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(
                It.IsAny<int>(),
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
            Code = 1,
            Name = string.Empty,
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.CreateProduct(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws(new ArgumentException("Credenciales invalidas"));

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
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Returns("Actualizado con exito");

        var result = _controller.UpdateProduct(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual("Actualizado con exito", result.Value);
    }

    [TestMethod]
    public void UpdateProduct_InvalidData_Returns400()
    {
        var request = new UpdateProductRequestModel
        {
            Name = string.Empty,  // dato inválido
            Description = "menos crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "nuevas-imagenes",
            Active = true
        };

        _prodServiceMock
            .Setup(s => s.UpdateProduct(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws(new ArgumentException("El nombre no puede estar vacío"));

        var result = _controller.UpdateProduct(1, request) as BadRequestResult;

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
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
            .Throws(new KeyNotFoundException());

        var result = _controller.UpdateProduct(999, request) as NotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }
}

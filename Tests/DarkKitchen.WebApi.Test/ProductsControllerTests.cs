using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test;

[TestClass]
public class ProductsControllerTests
{
    private Mock<IProductService> _prodServiceMock = null!;
    private ProdController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _prodServiceMock = new Mock<IProductService>();
        _controller = new ProdController(_prodServiceMock.Object);
    }

    [TestMethod]
    public void CreateProduct_ValidData_Returns201()
    {
        // Arrange
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
}

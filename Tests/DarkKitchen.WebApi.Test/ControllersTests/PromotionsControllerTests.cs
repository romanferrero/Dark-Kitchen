using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class PromotionsControllerTests
{
    private Mock<IPromotionService> _promServiceMock = null!;
    private PromotionsController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _promServiceMock = new Mock<IProductService>(MockBehavior.Strict);
        _controller = new ProductsController(_promServiceMock.Object);
    }

    [TestMethod]
    public void CreatePromotion_ValidData_Returns201()
    {
        var request = new CreatePromotionRequestModel
        {
            Code = 1,
            Name = "papas fritas",
            Description = "crujientes",
            Line = "snacks",
            Category = "frituras",
            Images = "imagenes",
            Active = true
        };

        _promServiceMock
            .Setup(s => s.CreatePromotion(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Returns("Creado con exito");

        var result = _controller.CreatePromotion(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual("Creado con exito", result.Value);
    }
}

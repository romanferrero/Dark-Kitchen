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
        _promServiceMock = new Mock<IPromotionService>(MockBehavior.Strict);
        _controller = new PromotionsController(_promServiceMock.Object);
    }

    [TestMethod]
    public void CreatePromotion_ValidData_Returns201()
    {
        var request = new CreatePromotionRequestModel
        {
            Name = "nombre",
            Discount = 10,
            DateFrom = DateOnly.FromDateTime(DateTime.Now),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
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

    [TestMethod]
    public void CreatePromotion_InvalidData_Returns400()
    {
        var request = new CreatePromotionRequestModel
        {
            Name = "nombre",
            Discount = 10,
            DateFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            DateTo = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        _promServiceMock
            .Setup(s => s.CreatePromotion(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Throws(new ArgumentException());

        var result = _controller.CreatePromotion(request) as BadRequestResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
}

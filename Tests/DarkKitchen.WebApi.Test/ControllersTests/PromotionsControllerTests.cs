using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
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
            Name = "Black Friday",
            Discount = 10,
            DateFrom = new DateOnly(2026, 5, 1),
            DateTo = new DateOnly(2026, 5, 31),
        };

        _promServiceMock
            .Setup(s => s.CreatePromotion(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Returns("Promotion created successfully.");

        var result = _controller.CreatePromotion(request) as CreatedAtActionResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
    }

    [TestMethod]
    public void CreatePromotion_InvalidData_Returns400()
    {
        var request = new CreatePromotionRequestModel
        {
            Name = string.Empty,
            Discount = 10,
            DateFrom = new DateOnly(2026, 5, 1),
            DateTo = new DateOnly(2026, 5, 31),
        };

        _promServiceMock
            .Setup(s => s.CreatePromotion(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
            .Throws(new ArgumentException());

        Assert.ThrowsException<ArgumentException>(() => _controller.CreatePromotion(request));
    }

    [TestMethod]
    public void GetPromotions_NoFilters_Returns200WithList()
    {
        var promotions = new List<Promotion>
        {
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)),
        };

        _promServiceMock
            .Setup(s => s.GetPromotions(null, null, null))
            .Returns(promotions);

        var result = _controller.GetPromotions(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetPromotions_WithValidDateFilter_Returns200()
    {
        var date = new DateOnly(2026, 5, 15);
        var promotions = new List<Promotion>
        {
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)),
        };

        _promServiceMock
            .Setup(s => s.GetPromotions(date, null, null))
            .Returns(promotions);

        var result = _controller.GetPromotions("2026-05-15", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetPromotions_WithInvalidDateFilter_PassesNullDate()
    {
        _promServiceMock
            .Setup(s => s.GetPromotions(null, null, null))
            .Returns([]);

        var result = _controller.GetPromotions("not-a-date", null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdatePromotion_ValidData_Returns200()
    {
        var request = new UpdatePromotionRequestModel
        {
            Name = "Cyber Monday",
            Discount = 25,
            DateFrom = new DateOnly(2026, 6, 1),
            DateTo = new DateOnly(2026, 6, 7),
        };

        _promServiceMock
            .Setup(s => s.UpdatePromotion(1, "Cyber Monday", 25, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 7)))
            .Returns("Promotion updated successfully.");

        var result = _controller.UpdatePromotion(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void UpdatePromotion_InvalidData_Returns400()
    {
        var request = new UpdatePromotionRequestModel
        {
            Name = string.Empty,
            Discount = 0,
            DateFrom = new DateOnly(2026, 6, 1),
            DateTo = new DateOnly(2026, 6, 7),
        };

        _promServiceMock
            .Setup(s => s.UpdatePromotion(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Throws(new ArgumentException());

        var result = _controller.UpdatePromotion(1, request) as BadRequestResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void UpdatePromotion_NotFound_Returns404()
    {
        var request = new UpdatePromotionRequestModel
        {
            Name = "Cyber Monday",
            Discount = 25,
            DateFrom = new DateOnly(2026, 6, 1),
            DateTo = new DateOnly(2026, 6, 7),
        };

        _promServiceMock
            .Setup(s => s.UpdatePromotion(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
            .Throws(new KeyNotFoundException());

        var result = _controller.UpdatePromotion(99, request) as NotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public void AddProduct_ValidData_Returns200()
    {
        var request = new AddProductToPromotionRequestModel { ProductCode = "BURG01" };

        _promServiceMock
            .Setup(s => s.AddProduct(1, "BURG01"))
            .Returns("Product added to promotion successfully.");

        var result = _controller.AddProduct(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void AddProduct_NotFound_Returns404()
    {
        var request = new AddProductToPromotionRequestModel { ProductCode = "NOEXISTE" };

        _promServiceMock
            .Setup(s => s.AddProduct(It.IsAny<int>(), It.IsAny<string>()))
            .Throws(new KeyNotFoundException());

        var result = _controller.AddProduct(1, request) as NotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [TestMethod]
    public void AddProduct_Duplicate_Returns409()
    {
        var request = new AddProductToPromotionRequestModel { ProductCode = "BURG01" };

        _promServiceMock
            .Setup(s => s.AddProduct(It.IsAny<int>(), It.IsAny<string>()))
            .Throws(new InvalidOperationException("Product already associated."));

        var result = _controller.AddProduct(1, request) as ConflictObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(409, result.StatusCode);
    }

    [TestMethod]
    public void RemoveProduct_ValidData_Returns200()
    {
        _promServiceMock
            .Setup(s => s.RemoveProduct(1, "BURG01"))
            .Returns("Product removed from promotion successfully.");

        var result = _controller.RemoveProduct(1, "BURG01") as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void RemoveProduct_NotFound_Returns404()
    {
        _promServiceMock
            .Setup(s => s.RemoveProduct(It.IsAny<int>(), It.IsAny<string>()))
            .Throws(new KeyNotFoundException());

        var result = _controller.RemoveProduct(99, "NOEXISTE") as NotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }
}

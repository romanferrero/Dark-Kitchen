using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.PromotionsControllers;
using DarkKitchen.WebApi.Models.Request.PromotionsModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.PromotionsControllersTests;

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

    private static PromotionExitDTO MakePromotionDTO(string name = "Black Friday", int discount = 10)
    {
        return new PromotionExitDTO
        {
            Id = 1,
            Name = name,
            DiscountPercentage = discount,
            DateFrom = new DateOnly(2026, 5, 1),
            DateTo = new DateOnly(2026, 5, 31),
            Products = []
        };
    }

    private static ProductExitDTO MakeProductDTO(string code = "BURG01", string name = "Hamburguesa clasica")
    {
        return new ProductExitDTO
        {
            Code = code,
            Name = name,
            Price = 100m,
            Line = "Combo burgers",
            Category = "Parrilla",
            ImageUrls = []
        };
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
            .Returns(MakePromotionDTO());

        var result = _controller.CreatePromotion(request) as CreatedResult;

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
        _promServiceMock
            .Setup(s => s.GetPromotions(null, null, null))
            .Returns([MakePromotionDTO()]);

        var result = _controller.GetPromotions(null, null, null) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetPromotions_WithValidDateFilter_Returns200()
    {
        var date = new DateOnly(2026, 5, 15);

        _promServiceMock
            .Setup(s => s.GetPromotions(date, null, null))
            .Returns([MakePromotionDTO()]);

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
            .Returns(MakePromotionDTO("Cyber Monday", 25));

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

        Assert.ThrowsException<ArgumentException>(() => _controller.UpdatePromotion(1, request));
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

        Assert.ThrowsException<KeyNotFoundException>(() => _controller.UpdatePromotion(99, request));
    }

    [TestMethod]
    public void AddProduct_ValidData_Returns200()
    {
        var request = new AddProductToPromotionRequestModel { ProductCode = "BURG01" };

        _promServiceMock
            .Setup(s => s.AddProduct(1, "BURG01"))
            .Returns(MakeProductDTO("BURG01"));

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

        Assert.ThrowsException<KeyNotFoundException>(() => _controller.AddProduct(1, request));
    }

    [TestMethod]
    public void AddProduct_Duplicate_Returns409()
    {
        var request = new AddProductToPromotionRequestModel { ProductCode = "BURG01" };

        _promServiceMock
            .Setup(s => s.AddProduct(It.IsAny<int>(), It.IsAny<string>()))
            .Throws(new InvalidOperationException());

        Assert.ThrowsException<InvalidOperationException>(() => _controller.AddProduct(1, request));
    }

    [TestMethod]
    public void RemoveProduct_ValidData_Returns200()
    {
        _promServiceMock
            .Setup(s => s.RemoveProduct(1, "BURG01"))
            .Returns(MakeProductDTO("BURG01"));

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

        Assert.ThrowsException<KeyNotFoundException>(() => _controller.RemoveProduct(99, "NOEXISTE"));
    }
}

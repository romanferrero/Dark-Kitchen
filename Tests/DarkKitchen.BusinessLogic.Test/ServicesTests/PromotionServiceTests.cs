using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class PromotionServiceTests
{
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IAuditLogRepository> _auditRepoMock = null!;
    private PromotionService _promotionService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _auditRepoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        _promotionService = new PromotionService(_promotionRepoMock.Object, _productRepoMock.Object, _auditRepoMock.Object);
    }

    [TestMethod]
    public void CreatePromotion_ValidData_CallsRepositoryAdd()
    {
        var dto = new CreatePromotionEntryDto(
            "Black Friday",
            10,
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 31));

        _promotionRepoMock.Setup(r => r.Add(It.IsAny<Promotion>()));
        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.CreatePromotion(dto, "admin@darkkitchen.com");

        _promotionRepoMock.Verify(r => r.Add(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void CreatePromotion_ValidData_AddsAuditLogWithCorrectData()
    {
        var dto = new CreatePromotionEntryDto(
            "Black Friday",
            10,
            new DateOnly(2026, 5, 1),
            new DateOnly(2026, 5, 31));

        _promotionRepoMock.Setup(r => r.Add(It.IsAny<Promotion>()));
        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.CreatePromotion(dto, "admin@darkkitchen.com");

        _auditRepoMock.Verify(
            r => r.Add(It.Is<AuditLog>(a =>
                a.EntityName == "PROMOTION" &&
                a.Description == "Creation" &&
                a.ResponsibleUser == "admin@darkkitchen.com")),
            Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_ValidData_CallsRepositoryUpdate()
    {
        var existing = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        existing.Id = 1;

        var dto = new UpdatePromotionEntryDto(
            1,
            "Cyber Monday",
            25,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 7));

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(existing);

        _promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()));

        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.UpdatePromotion(dto, "admin@darkkitchen.com");

        _promotionRepoMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_ValidData_AddsAuditLogWithCorrectData()
    {
        var existing = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        existing.Id = 1;

        var dto = new UpdatePromotionEntryDto(
            1,
            "Cyber Monday",
            25,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 7));

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(existing);

        _promotionRepoMock.Setup(r => r.Update(It.IsAny<Promotion>()));
        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.UpdatePromotion(dto, "admin@darkkitchen.com");

        _auditRepoMock.Verify(
            r => r.Add(It.Is<AuditLog>(a =>
                a.EntityName == "PROMOTION" &&
                a.Description == "Modification" &&
                a.ResponsibleUser == "admin@darkkitchen.com")),
            Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_NotFound_ThrowsKeyNotFoundException()
    {
        var dto = new UpdatePromotionEntryDto(
            99,
            "Cyber Monday",
            25,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 7));

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns((Promotion?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.UpdatePromotion(dto, "admin@darkkitchen.com"));
    }

    [TestMethod]
    public void AddProduct_ValidData_CallsRepositoryUpdate()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        promotion.Id = 1;

        var product = Product.Create(
            "BURG01",
            "Hamburguesa clasica",
            100m,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/b.jpg",
            true);

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _productRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()));

        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.AddProduct(1, "BURG01", "admin@darkkitchen.com");

        _promotionRepoMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void RemoveProduct_ValidData_CallsRepositoryUpdate()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        promotion.Id = 1;

        var product = Product.Create(
            "BURG01",
            "Hamburguesa clasica",
            100m,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/b.jpg",
            true);

        promotion.AddProduct(product);

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()));

        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.RemoveProduct(1, "BURG01", "admin@darkkitchen.com");

        _promotionRepoMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void GetPromotions_DelegatesToRepository()
    {
        var promotions = new List<Promotion>
        {
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31))
        };

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns(promotions);

        var result = _promotionService.GetPromotions(null, null, null);

        Assert.AreEqual(1, result.Count);
        _promotionRepoMock.Verify(
            r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()),
            Times.Once);
    }

    private static Product BuildProduct(string code, string line = "Combo burgers", string name = "Hamburguesa clasica") =>
        Product.Create(
            code,
            name,
            100m,
            "Hamburguesa con queso y lechuga fresca",
            line,
            "Parrilla",
            "http://img.com/test.jpg|100",
            true);

    private static Promotion BuildPromotionWith(params Product[] products)
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        foreach(var product in products)
        {
            promotion.AddProduct(product);
        }

        return promotion;
    }

    [TestMethod]
    public void GetPromotions_DateInRange_MatchesPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith()]);

        var result = _promotionService.GetPromotions(new DateOnly(2026, 5, 15), null, null);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetPromotions_DateOutOfRange_FiltersOutPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith()]);

        var result = _promotionService.GetPromotions(new DateOnly(2026, 12, 1), null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetPromotions_LineMatchesAtLeastOneProduct_MatchesPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith(BuildProduct("BURG01", line: "Combo burgers"))]);

        var result = _promotionService.GetPromotions(null, "Combo burgers", null);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetPromotions_LineMatchesNoProduct_FiltersOutPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith(BuildProduct("BURG01", line: "Combo burgers"))]);

        var result = _promotionService.GetPromotions(null, "Linea inexistente", null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetPromotions_ProductMatchesByCode_MatchesPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith(BuildProduct("BURG01"))]);

        var result = _promotionService.GetPromotions(null, null, "BURG01");

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetPromotions_ProductMatchesByName_MatchesPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith(BuildProduct("BURG01", name: "Hamburguesa especial"))]);

        var result = _promotionService.GetPromotions(null, null, "hamburguesa");

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetPromotions_ProductMatchesNothing_FiltersOutPromotion()
    {
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<Expression<Func<Promotion, bool>>?>()))
            .Returns([BuildPromotionWith(BuildProduct("BURG01"))]);

        var result = _promotionService.GetPromotions(null, null, "NOEXISTE");

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void AddProduct_ValidData_AddsAuditLogWithCorrectData()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        promotion.Id = 1;

        var product = Product.Create(
            "BURG01",
            "Hamburguesa clasica",
            100m,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/b.jpg",
            true);

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _productRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(product);

        _promotionRepoMock.Setup(r => r.Update(It.IsAny<Promotion>()));
        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.AddProduct(1, "BURG01", "admin@darkkitchen.com");

        _auditRepoMock.Verify(
            r => r.Add(It.Is<AuditLog>(a =>
                a.EntityName == "PROMOCION" &&
                a.Description == "Asociación de producto BURG01" &&
                a.ResponsibleUser == "admin@darkkitchen.com")),
            Times.Once);
    }

    [TestMethod]
    public void AddProduct_PromotionNotFound_ThrowsKeyNotFoundException()
    {
        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns((Promotion?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.AddProduct(99, "BURG01", "admin@darkkitchen.com"));
    }

    [TestMethod]
    public void AddProduct_ProductNotFound_ThrowsKeyNotFoundException()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        promotion.Id = 1;

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _productRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns((Product?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.AddProduct(1, "NOEXISTE", "admin@darkkitchen.com"));
    }

    [TestMethod]
    public void RemoveProduct_ValidData_AddsAuditLogWithCorrectData()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        promotion.Id = 1;

        var product = Product.Create(
            "BURG01",
            "Hamburguesa clasica",
            100m,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/b.jpg",
            true);

        promotion.AddProduct(product);

        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotion);

        _promotionRepoMock.Setup(r => r.Update(It.IsAny<Promotion>()));
        _auditRepoMock.Setup(r => r.Add(It.IsAny<AuditLog>()));

        _promotionService.RemoveProduct(1, "BURG01", "admin@darkkitchen.com");

        _auditRepoMock.Verify(
            r => r.Add(It.Is<AuditLog>(a =>
                a.EntityName == "PROMOCION" &&
                a.Description == "Eliminación de producto BURG01" &&
                a.ResponsibleUser == "admin@darkkitchen.com")),
            Times.Once);
    }

    [TestMethod]
    public void RemoveProduct_PromotionNotFound_ThrowsKeyNotFoundException()
    {
        _promotionRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns((Promotion?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.RemoveProduct(99, "BURG01", "admin@darkkitchen.com"));
    }
}

using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class PromotionServiceTests
{
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private PromotionService _promotionService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _promotionService = new PromotionService(_promotionRepoMock.Object, _productRepoMock.Object);
    }

    [TestMethod]
    public void CreatePromotion_ValidData_CallsRepositoryAdd()
    {
        _promotionRepoMock.Setup(r => r.Add(It.IsAny<Promotion>()));

        _promotionService.CreatePromotion("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        _promotionRepoMock.Verify(r => r.Add(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_ValidData_CallsRepositoryUpdate()
    {
        var existing = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        _promotionRepoMock.Setup(r => r.GetById(1)).Returns(existing);
        _promotionRepoMock.Setup(r => r.Update(It.IsAny<Promotion>()));

        _promotionService.UpdatePromotion(1, "Cyber Monday", 25, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 7));

        _promotionRepoMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void UpdatePromotion_NotFound_ThrowsKeyNotFoundException()
    {
        _promotionRepoMock.Setup(r => r.GetById(99)).Returns((Promotion?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _promotionService.UpdatePromotion(99, "Cyber Monday", 25, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 7)));
    }

    [TestMethod]
    public void AddProduct_ValidData_CallsRepositoryUpdate()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = Product.Create("BURG01", "Hamburguesa clasica", "Hamburguesa con lechuga y tomate fresco", "Combo burgers", "Parrilla", "http://img.com/b.jpg", true);

        _promotionRepoMock.Setup(r => r.GetById(1)).Returns(promotion);
        _productRepoMock.Setup(r => r.GetByCode("BURG01")).Returns(product);
        _promotionRepoMock.Setup(r => r.Update(It.IsAny<Promotion>()));

        _promotionService.AddProduct(1, "BURG01");

        _promotionRepoMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }

    [TestMethod]
    public void RemoveProduct_ValidData_CallsRepositoryUpdate()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = Product.Create("BURG01", "Hamburguesa clasica", "Hamburguesa con lechuga y tomate fresco", "Combo burgers", "Parrilla", "http://img.com/b.jpg", true);
        promotion.AddProduct(product);

        _promotionRepoMock.Setup(r => r.GetById(1)).Returns(promotion);
        _promotionRepoMock.Setup(r => r.Update(It.IsAny<Promotion>()));

        _promotionService.RemoveProduct(1, "BURG01");

        _promotionRepoMock.Verify(r => r.Update(It.IsAny<Promotion>()), Times.Once);
    }
}

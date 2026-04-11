using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class PromotionServiceTests
{
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private PromotionService _promotionService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _promotionService = new PromotionService(_promotionRepoMock.Object);
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
}

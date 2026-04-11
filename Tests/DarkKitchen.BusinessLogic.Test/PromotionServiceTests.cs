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
}

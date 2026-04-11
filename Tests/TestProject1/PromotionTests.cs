namespace DarkKitchen.Domain.Test;

[TestClass]
public class PromotionTests
{
    [TestMethod]
    public void Create_ValidData_ReturnsPromotion()
    {
        var dateFrom = new DateOnly(2026, 5, 1);
        var dateTo = new DateOnly(2026, 5, 31);

        var promotion = Promotion.Create("Black Friday", 20, dateFrom, dateTo);

        Assert.AreEqual("Black Friday", promotion.Name);
        Assert.AreEqual(20, promotion.DiscountPercentage);
        Assert.AreEqual(dateFrom, promotion.DateFrom);
        Assert.AreEqual(dateTo, promotion.DateTo);
    }

    [TestMethod]
    public void Create_EmptyName_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create(string.Empty, 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)));
    }

    [TestMethod]
    public void Create_DiscountZero_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create("Black Friday", 0, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)));
    }
}

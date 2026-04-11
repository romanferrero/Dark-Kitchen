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

    [TestMethod]
    public void Create_DateToBeforeDateFrom_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 31), new DateOnly(2026, 5, 1)));
    }

    [TestMethod]
    public void AddProduct_ValidProduct_AddsToList()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = Product.Create("BURG01", "Hamburguesa clasica", "Hamburguesa con lechuga y tomate fresco", "Combo burgers", "Parrilla", "http://img.com/b.jpg", true);

        promotion.AddProduct(product);

        Assert.AreEqual(1, promotion.Products.Count);
        Assert.AreEqual("BURG01", promotion.Products[0].Code);
    }
}

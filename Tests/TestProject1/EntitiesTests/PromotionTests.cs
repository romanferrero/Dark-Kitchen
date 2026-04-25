using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.EntitiesTests;

[TestClass]
public class PromotionTests
{
    private const decimal ValidPrice = 100;

    private Product CreateProduct(string code = "BURG01")
    {
        return Product.Create(
            code,
            "Hamburguesa clasica",
            ValidPrice,
            "Hamburguesa con lechuga y tomate fresco",
            "Combo burgers",
            "Parrilla",
            "http://img.com/b.jpg",
            true);
    }

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
    public void Create_DiscountGreaterThan100_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create("Black Friday", 150, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)));
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
        var product = CreateProduct();

        promotion.AddProduct(product);

        Assert.AreEqual(1, promotion.Products.Count);
        Assert.AreEqual(product.Code, promotion.Products[0].Code);
    }

    [TestMethod]
    public void AddProduct_DuplicateProduct_ThrowsInvalidOperationException()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = CreateProduct();

        promotion.AddProduct(product);

        Assert.ThrowsException<InvalidOperationException>(() => promotion.AddProduct(product));
    }

    [TestMethod]
    public void RemoveProduct_ExistingProduct_RemovesFromList()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = CreateProduct();

        promotion.AddProduct(product);
        promotion.RemoveProduct(product.Code);

        Assert.AreEqual(0, promotion.Products.Count);
    }

    [TestMethod]
    public void RemoveProduct_NonExistingProduct_ThrowsKeyNotFoundException()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        Assert.ThrowsException<KeyNotFoundException>(() => promotion.RemoveProduct("NOEXISTE"));
    }

    [TestMethod]
    public void Update_ValidData_UpdatesPromotion()
    {
        var promotion = Promotion.Create("Old", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        var newFrom = new DateOnly(2026, 6, 1);
        var newTo = new DateOnly(2026, 6, 30);

        promotion.Update("New Promo", 25, newFrom, newTo);

        Assert.AreEqual("New Promo", promotion.Name);
        Assert.AreEqual(25, promotion.DiscountPercentage);
        Assert.AreEqual(newFrom, promotion.DateFrom);
        Assert.AreEqual(newTo, promotion.DateTo);
    }

    [TestMethod]
    public void Update_InvalidDates_ThrowsArgumentException()
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        Assert.ThrowsException<ArgumentException>(() =>
            promotion.Update("Promo", 10, new DateOnly(2026, 6, 30), new DateOnly(2026, 6, 1)));
    }
}

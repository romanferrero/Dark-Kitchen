using DarkKitchen.Domain.Entities;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class PromotionTests
{
    private static Product BuildProduct(string code) =>
        Product.Create(new CreateProductParams(
            code,
            "Producto valido de prueba",
            100m,
            "Descripcion suficientemente larga para pasar la validacion",
            "Combo burgers",
            "Parrilla",
            "http://img.com/test.jpg|100",
            true));

    [TestMethod]
    public void Create_DateToBeforeDateFrom_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create(
                "Promo",
                10,
                new DateOnly(2026, 5, 10),
                new DateOnly(2026, 5, 1)));
    }

    [TestMethod]
    public void Create_EmptyName_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create(
                string.Empty,
                10,
                new DateOnly(2026, 5, 1),
                new DateOnly(2026, 5, 31)));
    }

    [TestMethod]
    public void Create_DiscountBelowMin_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create(
                "Promo",
                0,
                new DateOnly(2026, 5, 1),
                new DateOnly(2026, 5, 31)));
    }

    [TestMethod]
    public void Create_DiscountAboveMax_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Promotion.Create(
                "Promo",
                101,
                new DateOnly(2026, 5, 1),
                new DateOnly(2026, 5, 31)));
    }

    [TestMethod]
    public void AddProduct_DuplicateProduct_Throws()
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = BuildProduct("BURG01");

        promotion.AddProduct(product);

        Assert.ThrowsException<InvalidOperationException>(() => promotion.AddProduct(product));
    }

    [TestMethod]
    public void RemoveProduct_NotAssociated_Throws()
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        Assert.ThrowsException<KeyNotFoundException>(() => promotion.RemoveProduct("NOEXISTE"));
    }

    [TestMethod]
    public void RemoveProduct_Associated_RemovesIt()
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var product = BuildProduct("BURG01");
        promotion.AddProduct(product);

        promotion.RemoveProduct("BURG01");

        Assert.AreEqual(0, promotion.Products.Count);
    }

    [TestMethod]
    public void Update_DateToBeforeDateFrom_Throws()
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        Assert.ThrowsException<ArgumentException>(() =>
            promotion.Update("Promo2", 20, new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 1)));
    }

    [TestMethod]
    public void Update_ValidData_UpdatesFields()
    {
        var promotion = Promotion.Create("Promo", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        promotion.Update("Promo2", 25, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30));

        Assert.AreEqual("Promo2", promotion.Name);
        Assert.AreEqual(25, promotion.DiscountPercentage);
    }
}

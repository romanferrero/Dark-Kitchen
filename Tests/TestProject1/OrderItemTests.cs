using DarkKitchen.Domain;

namespace DarkKitchen.Domain.Test;

[TestClass]
public class OrderItemTests
{
    private Product _product = null!;

    [TestInitialize]
    public void Initialize()
    {
        _product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);
    }

    [TestMethod]
    public void Create_ShouldCreateOrderItem_WhenDataIsValid()
    {
        var item = OrderItem.Create(
            _product,
            2,
            100,
            80,
            "Promo",
            20);

        Assert.IsNotNull(item);
        Assert.AreEqual(_product.Id, item.ProductId);
        Assert.AreEqual(_product, item.Product);
        Assert.AreEqual(2, item.Quantity);
        Assert.AreEqual(100, item.OriginalPrice);
        Assert.AreEqual(80, item.UnitPrice);
        Assert.AreEqual("Promo", item.PromotionName);
        Assert.AreEqual(20, item.DiscountPercentage);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_ShouldThrowException_WhenQuantityIsZero()
    {
        OrderItem.Create(_product, 0, 100, 80, null, null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_ShouldThrowException_WhenQuantityIsNegative()
    {
        OrderItem.Create(_product, -1, 100, 80, null, null);
    }
}

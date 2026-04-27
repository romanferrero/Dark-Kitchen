using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.EntitiesTests;

[TestClass]
public class OrderProductTests
{
    [TestMethod]
    public void Constructor_ShouldCreateOrderProduct()
    {
        OrderProduct orderProduct = new OrderProduct();

        Assert.IsNotNull(orderProduct);
    }

    [TestMethod]
    public void OrderId_ShouldSetAndGetCorrectly()
    {
        OrderProduct orderProduct = new OrderProduct();

        orderProduct.OrderId = 1;

        Assert.AreEqual(1, orderProduct.OrderId);
    }

    [TestMethod]
    public void ProductId_ShouldSetAndGetCorrectly()
    {
        OrderProduct orderProduct = new OrderProduct();

        orderProduct.ProductId = 10;

        Assert.AreEqual(10, orderProduct.ProductId);
    }

    [TestMethod]
    public void Product_ShouldSetAndGetCorrectly()
    {
        OrderProduct orderProduct = new OrderProduct();
        Product product = new Product();

        orderProduct.Product = product;

        Assert.AreEqual(product, orderProduct.Product);
    }

    [TestMethod]
    public void Quantity_ShouldSetAndGetCorrectly_WhenValueIsOne()
    {
        OrderProduct orderProduct = new OrderProduct();

        orderProduct.Quantity = 1;

        Assert.AreEqual(1, orderProduct.Quantity);
    }

    [TestMethod]
    public void Quantity_ShouldSetAndGetCorrectly_WhenValueIsGreaterThanOne()
    {
        OrderProduct orderProduct = new OrderProduct();

        orderProduct.Quantity = 5;

        Assert.AreEqual(5, orderProduct.Quantity);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Quantity_ShouldThrowArgumentException_WhenValueIsZero()
    {
        OrderProduct orderProduct = new OrderProduct();

        orderProduct.Quantity = 0;
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Quantity_ShouldThrowArgumentException_WhenValueIsNegative()
    {
        OrderProduct orderProduct = new OrderProduct();

        orderProduct.Quantity = -3;
    }

    [TestMethod]
    public void Quantity_ShouldThrowArgumentException_WithCorrectMessage_WhenValueIsZero()
    {
        OrderProduct orderProduct = new OrderProduct();

        ArgumentException exception = Assert.ThrowsException<ArgumentException>(() =>
        {
            orderProduct.Quantity = 0;
        });

        Assert.AreEqual("Quantity must be at least 1.", exception.Message);
    }
}

using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Test.EntitiesTests;

[TestClass]
public class OrderTests
{
    private int _orderId;
    private Product _product = null!;
    private Address _address = null!;
    private DeliveryType _deliveryType;

    [TestInitialize]
    public void Initialize()
    {
        _orderId = 1;
        _deliveryType = DeliveryType.Express;
        _address = Address.Create("Calle Principal", "11", "001");

        _product = Product.Create(
            "PROD01",
            "Producto 1 valido",
            100,
            "Descripción del producto 1 suficientemente larga",
            "Línea A",
            "Categoría B",
            "imagen1.jpg|100,imagen2.jpg|200",
            true);
    }

    private Order BuildValidOrder()
    {
        return Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product> { _product },
            100,
            1001,
            10,
            2,
            12);
    }

    [TestMethod]
    public void CreateOrder_Valid()
    {
        var beforeCreation = DateTime.Now;

        var order = Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product> { _product },
            100,
            1001,
            36.49m,
            5.99m,
            42.48m);

        var afterCreation = DateTime.Now;

        Assert.IsNotNull(order);
        Assert.AreEqual(_orderId, order.OrderId);
        Assert.AreEqual(_deliveryType, order.DeliveryType);
        Assert.AreEqual(_address, order.Address);
        Assert.AreEqual(1, order.Products.Count);
        Assert.AreEqual(OrderStatus.Pending, order.OrderStatus);
        Assert.AreEqual(100, order.ClientId);
        Assert.AreEqual(1001, order.OrderNumber);
        Assert.AreEqual(36.49m, order.Subtotal);
        Assert.AreEqual(5.99m, order.ShippingCost);
        Assert.AreEqual(42.48m, order.TotalCost);

        Assert.IsTrue(order.OrderDate >= beforeCreation);
        Assert.IsTrue(order.OrderDate <= afterCreation);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrder_Invalid_OrderId()
    {
        Order.Create(
            -1,
            _deliveryType,
            _address,
            new List<Product> { _product },
            100,
            1001,
            10,
            2,
            12);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrder_Empty_Products()
    {
        Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product>(),
            100,
            1001,
            10,
            2,
            12);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrder_Invalid_ClientId()
    {
        Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product> { _product },
            -1,
            1001,
            10,
            2,
            12);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrder_Invalid_OrderNumber()
    {
        Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product> { _product },
            100,
            -1,
            10,
            2,
            12);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrder_Invalid_Subtotal()
    {
        Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product> { _product },
            100,
            1001,
            -1,
            2,
            12);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrder_Invalid_TotalCost()
    {
        Order.Create(
            _orderId,
            _deliveryType,
            _address,
            new List<Product> { _product },
            100,
            1001,
            10,
            2,
            -1);
    }

    [TestMethod]
    public void UpdateOrderStatus_PendingToPrepared_Valid()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Prepared);

        Assert.AreEqual(OrderStatus.Prepared, order.OrderStatus);
    }

    [TestMethod]
    public void UpdateOrderStatus_PendingToCancelled_Valid()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Cancelled);

        Assert.AreEqual(OrderStatus.Cancelled, order.OrderStatus);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrderStatus_Cancel_Invalid_WhenNotPending()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Prepared);
        order.UpdateStatus(OrderStatus.Cancelled);
    }

    [TestMethod]
    public void UpdateOrderStatus_PreparedToOnTheWay_Valid()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Prepared);
        order.UpdateStatus(OrderStatus.OnTheWay);

        Assert.AreEqual(OrderStatus.OnTheWay, order.OrderStatus);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrderStatus_OnTheWay_Invalid_WhenNotPrepared()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.OnTheWay);
    }

    [TestMethod]
    public void UpdateOrderStatus_OnTheWayToDelivered_Valid()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Prepared);
        order.UpdateStatus(OrderStatus.OnTheWay);
        order.UpdateStatus(OrderStatus.Delivered);

        Assert.AreEqual(OrderStatus.Delivered, order.OrderStatus);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrderStatus_Delivered_Invalid_WhenNotOnTheWay()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Delivered);
    }

    [TestMethod]
    public void UpdateOrderStatus_OnTheWayToNotDelivered_Valid()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.Prepared);
        order.UpdateStatus(OrderStatus.OnTheWay);
        order.UpdateStatus(OrderStatus.NotDelivered);

        Assert.AreEqual(OrderStatus.NotDelivered, order.OrderStatus);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void UpdateOrderStatus_NotDelivered_Invalid_WhenNotOnTheWay()
    {
        var order = BuildValidOrder();

        order.UpdateStatus(OrderStatus.NotDelivered);
    }
}

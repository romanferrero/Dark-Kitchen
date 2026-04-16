namespace DarkKitchen.Domain.Test;

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
        _product = Product.Create("PROD01", "Producto 1", "Descripción del producto 1", "Línea A", "Categoría B",
            "imagen1.jpg|100,imagen2.jpg|200", true);
    }

    private Order BuildValidOrder()
    {
        var products = new List<Product> { _product };

        return Order.Create(
            _orderId,
            _deliveryType,
            _address,
            products,
            100,
            1001,
            10,
            2,
            12);
    }

    [TestMethod]
    public void CreateOrder_Valid()
    {
        var products = new List<Product>
        {
            _product
                    };
        var clientId = 100;
        var orderNumber = 1001;
        var subtotal = 36.49;
        var shippingCost = 5.99;
        var totalCost = 42.48;

        var order = Order.Create(
            _orderId,
            _deliveryType,
            _address,
            products,
            clientId,
            orderNumber,
            subtotal,
            shippingCost,
            totalCost);

        Assert.IsNotNull(order);
        Assert.AreEqual(_orderId, order.OrderId);
        Assert.AreEqual(_deliveryType, order.DeliveryType);
        Assert.AreEqual(_address, order.Address);
        Assert.AreEqual(products, order.Products);
        Assert.AreEqual(OrderStatus.Pending, order.OrderStatus);
        Assert.AreEqual(clientId, order.ClientId);
        Assert.AreEqual(orderNumber, order.OrderNumber);
        Assert.AreEqual(subtotal, order.Subtotal);
        Assert.AreEqual(shippingCost, order.ShippingCost);
        Assert.AreEqual(totalCost, order.TotalCost);
        Assert.IsNotNull(order.OrderDate);
        Assert.IsTrue(order.OrderDate <= DateTime.Now);
        Assert.IsTrue(order.OrderDate > DateTime.Now.AddSeconds(-1));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void CreateOrder_Invalid_OrderId()
    {
        Order.Create(
            -1,
            _deliveryType,
            _address,
            [_product],
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
            [],
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
            [_product],
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
            [_product],
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
            [_product],
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
            [_product],
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
    public void UpdateOrderStatus_PendingToPrepared_Invalid()
    {
        var order = BuildValidOrder();
        order.OrderStatus = OrderStatus.Delivered;

        order.UpdateStatus(OrderStatus.Prepared);

        Assert.AreEqual(OrderStatus.Prepared, order.OrderStatus);
    }
}

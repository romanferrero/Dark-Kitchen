using DarkKitchen.Domain;

namespace MyNamespace;

[TestClass]
public class OrderTests
{
    [TestMethod]
    public void OrderService_CreateOrder_Valid()
    {
        var orderId = 1;
        var deliveryType = DeliveryType.Express;

        var address = Address.Create("Calle Principal", "11", "001");
        var products = new List<Product>
        {
            Product.Create("PROD01", "Producto 1", "Descripción del producto 1", "Línea A", "Categoría B", "imagen1.jpg|100,imagen2.jpg|200", true)
        };
        var clientId = 100;
        var orderNumber = 1001;
        var subtotal = 36.49;
        var shippingCost = 5.99;
        var totalCost = 42.48;

        Order order = Order.Create(
            orderId,
            deliveryType,
            address,
            products,
            clientId,
            orderNumber,
            subtotal,
            shippingCost,
            totalCost);

        Assert.IsNotNull(order);
        Assert.AreEqual(orderId, order.OrderId);
        Assert.AreEqual(deliveryType, order.DeliveryType);
        Assert.AreEqual(address, order.Address);
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
}

using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.BusinessLogic.Test.EntitiesTests;

[TestClass]
public class OrderTests
{
    private static Order BuildOrder()
    {
        var product = Product.Create(
            "BURG01",
            "Hamburguesa clasica",
            100m,
            "Hamburguesa con queso y lechuga fresca",
            "Combo burgers",
            "Parrilla",
            "http://img.com/test.jpg|100",
            true);

        return Order.Create(
            DeliveryType.Express,
            Address.Create("Calle", "123", "A"),
            [new OrderProduct { ProductId = product.Id, Product = product, Quantity = 1 }],
            clientId: 1,
            orderNumber: 100,
            subtotal: 100m,
            shippingCost: 20m,
            totalCost: 146.4m);
    }

    [TestMethod]
    public void Create_EmptyProductList_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            Order.Create(
                DeliveryType.Express,
                Address.Create("Calle", "123", "A"),
                [],
                clientId: 1,
                orderNumber: 100,
                subtotal: 0m,
                shippingCost: 0m,
                totalCost: 0m));
    }

    [TestMethod]
    public void UpdateStatus_PendingToPrepared_Succeeds()
    {
        var order = BuildOrder();

        order.UpdateStatus(OrderStatus.Prepared);

        Assert.AreEqual(OrderStatus.Prepared, order.OrderStatus);
    }

    [TestMethod]
    public void UpdateStatus_PendingToCancelled_Succeeds()
    {
        var order = BuildOrder();

        order.UpdateStatus(OrderStatus.Cancelled);

        Assert.AreEqual(OrderStatus.Cancelled, order.OrderStatus);
    }

    [TestMethod]
    public void UpdateStatus_PreparedToOnTheWay_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus(OrderStatus.Prepared);

        order.UpdateStatus(OrderStatus.OnTheWay);

        Assert.AreEqual(OrderStatus.OnTheWay, order.OrderStatus);
    }

    [TestMethod]
    public void UpdateStatus_OnTheWayToDelivered_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus(OrderStatus.Prepared);
        order.UpdateStatus(OrderStatus.OnTheWay);

        order.UpdateStatus(OrderStatus.Delivered);

        Assert.AreEqual(OrderStatus.Delivered, order.OrderStatus);
    }

    [TestMethod]
    public void UpdateStatus_OnTheWayToNotDelivered_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus(OrderStatus.Prepared);
        order.UpdateStatus(OrderStatus.OnTheWay);

        order.UpdateStatus(OrderStatus.NotDelivered);

        Assert.AreEqual(OrderStatus.NotDelivered, order.OrderStatus);
    }

    [TestMethod]
    public void UpdateStatus_PreparedFromNonPending_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus(OrderStatus.Prepared);

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus(OrderStatus.Prepared));
    }

    [TestMethod]
    public void UpdateStatus_CancelledFromNonPending_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus(OrderStatus.Prepared);

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus(OrderStatus.Cancelled));
    }

    [TestMethod]
    public void UpdateStatus_OnTheWayFromNonPrepared_Throws()
    {
        var order = BuildOrder();

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus(OrderStatus.OnTheWay));
    }

    [TestMethod]
    public void UpdateStatus_DeliveredFromNonOnTheWay_Throws()
    {
        var order = BuildOrder();

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus(OrderStatus.Delivered));
    }

    [TestMethod]
    public void UpdateStatus_NotDeliveredFromNonOnTheWay_Throws()
    {
        var order = BuildOrder();

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus(OrderStatus.NotDelivered));
    }

    [TestMethod]
    public void UpdateStatus_TargetNotInPolicyMap_DoesNotThrow()
    {
        var order = BuildOrder();

        order.UpdateStatus(OrderStatus.Pending);

        Assert.AreEqual(OrderStatus.Pending, order.OrderStatus);
    }
}

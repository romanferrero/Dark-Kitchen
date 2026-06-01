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

        order.UpdateStatus("Prepared");

        Assert.AreEqual("Prepared", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_PendingToCancelled_Succeeds()
    {
        var order = BuildOrder();

        order.UpdateStatus("Cancelled");

        Assert.AreEqual("Cancelled", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_PreparedToOnTheWay_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");

        order.UpdateStatus("OnTheWay");

        Assert.AreEqual("OnTheWay", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_OnTheWayToDelivered_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");
        order.UpdateStatus("OnTheWay");

        order.UpdateStatus("Delivered");

        Assert.AreEqual("Delivered", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_OnTheWayToNotDelivered_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");
        order.UpdateStatus("OnTheWay");

        order.UpdateStatus("NotDelivered");

        Assert.AreEqual("NotDelivered", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_PreparedFromNonPending_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Prepared"));
    }

    [TestMethod]
    public void UpdateStatus_CancelledFromNonPending_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Cancelled"));
    }

    [TestMethod]
    public void UpdateStatus_OnTheWayFromNonPrepared_Throws()
    {
        var order = BuildOrder();

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("OnTheWay"));
    }

    [TestMethod]
    public void UpdateStatus_DeliveredFromNonOnTheWay_Throws()
    {
        var order = BuildOrder();

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Delivered"));
    }

    [TestMethod]
    public void UpdateStatus_NotDeliveredFromNonOnTheWay_Throws()
    {
        var order = BuildOrder();

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("NotDelivered"));
    }

    [TestMethod]
    public void UpdateStatus_PendingToPending_DoesNotThrow()
    {
        var order = BuildOrder();

        order.UpdateStatus("Pending");

        Assert.AreEqual("Pending", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_PendingToDelayed_Succeeds()
    {
        var order = BuildOrder();

        order.UpdateStatus("Delayed");

        Assert.AreEqual("Delayed", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_DelayedToPrepared_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus("Delayed");

        order.UpdateStatus("Prepared");

        Assert.AreEqual("Prepared", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_DelayedToCancelled_Succeeds()
    {
        var order = BuildOrder();
        order.UpdateStatus("Delayed");

        order.UpdateStatus("Cancelled");

        Assert.AreEqual("Cancelled", order.State.Name);
    }

    [TestMethod]
    public void UpdateStatus_DelayedFromNonPending_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Delayed"));
    }

    [TestMethod]
    public void UpdateStatus_CancelledToAny_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus("Cancelled");

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Prepared"));
    }

    [TestMethod]
    public void UpdateStatus_DeliveredToAny_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");
        order.UpdateStatus("OnTheWay");
        order.UpdateStatus("Delivered");

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Prepared"));
    }

    [TestMethod]
    public void UpdateStatus_NotDeliveredToAny_Throws()
    {
        var order = BuildOrder();
        order.UpdateStatus("Prepared");
        order.UpdateStatus("OnTheWay");
        order.UpdateStatus("NotDelivered");

        Assert.ThrowsException<ArgumentException>(() => order.UpdateStatus("Prepared"));
    }
}

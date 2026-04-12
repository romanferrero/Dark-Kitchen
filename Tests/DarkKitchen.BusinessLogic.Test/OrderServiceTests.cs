using DarkKitchen.BusinessLogic.Interfaces;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _promotionRepoMock.Object);
    }

    [TestMethod]
    public void CreateOrder_ValidDataNoPromotion_ReturnsCorrectTotals()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);
        product.Price = 200m;

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)>
        {
            ("BURG01", 2),
        };

        var result = _orderService.CreateOrder(
            clientId: 1,
            deliveryType: "express",
            street: "18 de Julio",
            doorNumber: "1234",
            apartment: "Apto 101",
            items: items);

        Assert.AreEqual(1, result.ClientId);
        Assert.AreEqual(400m, result.Subtotal);
        Assert.IsTrue(result.ShippingCost > 0);
        Assert.IsTrue(result.Total > result.Subtotal);
    }

    [TestMethod]
    public void CreateOrder_EmptyItems_ThrowsArgumentException()
    {
        var items = new List<(string ProductCode, int Quantity)>();

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(
                clientId: 1,
                deliveryType: "express",
                street: "18 de Julio",
                doorNumber: "1234",
                apartment: "Apto 101",
                items: items));
    }

    [TestMethod]
    public void CreateOrder_InactiveProduct_ThrowsArgumentException()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: false);
        product.Price = 200m;

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        var items = new List<(string ProductCode, int Quantity)>
        {
            ("BURG01", 1),
        };

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(
                clientId: 1,
                deliveryType: "express",
                street: "18 de Julio",
                doorNumber: "1234",
                apartment: "Apto 101",
                items: items));
    }

    [TestMethod]
    public void CreateOrder_ProductNotFound_ThrowsKeyNotFoundException()
    {
        _productRepoMock
            .Setup(r => r.GetByCode("NOEXIST"))
            .Returns((Product?)null);

        var items = new List<(string ProductCode, int Quantity)>
        {
            ("NOEXIST", 1),
        };

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.CreateOrder(
                clientId: 1,
                deliveryType: "express",
                street: "18 de Julio",
                doorNumber: "1234",
                apartment: "Apto 101",
                items: items));
    }

    [TestMethod]
    public void CreateOrder_WithPromotion_AppliesHighestDiscount()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);
        product.Price = 1000m;

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        var promo10 = Promotion.Create("Promo 10", 10,
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        var promo20 = Promotion.Create("Promo 20", 20,
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([promo10, promo20]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)>
        {
            ("BURG01", 1),
        };

        var result = _orderService.CreateOrder(
            clientId: 1,
            deliveryType: "express",
            street: "18 de Julio",
            doorNumber: "1234",
            apartment: "Apto 101",
            items: items);

        Assert.AreEqual(800m, result.Subtotal);
    }

    [TestMethod]
    public void CreateOrder_ExpressDelivery_ReturnsExactShippingAndTotal()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);
        product.Price = 200m;

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)>
    {
        ("BURG01", 2),
    };

        var result = _orderService.CreateOrder(
            clientId: 1,
            deliveryType: "express",
            street: "18 de Julio",
            doorNumber: "1234",
            apartment: "Apto 101",
            items: items);

        Assert.AreEqual(400m, result.Subtotal);
        Assert.AreEqual(100m, result.ShippingCost);
        Assert.AreEqual(610m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_StandardDelivery_UsesStandardShippingCost()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);
        product.Price = 200m;

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)>
    {
        ("BURG01", 2),
    };

        var result = _orderService.CreateOrder(
            clientId: 1,
            deliveryType: "24hs",
            street: "18 de Julio",
            doorNumber: "1234",
            apartment: "Apto 101",
            items: items);

        Assert.AreEqual(400m, result.Subtotal);
        Assert.AreEqual(50m, result.ShippingCost);
        Assert.AreEqual(549m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_ValidData_PersistsPendingOrder()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: true);
        product.Price = 200m;

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)>
    {
        ("BURG01", 1),
    };

        _orderService.CreateOrder(
            clientId: 1,
            deliveryType: "express",
            street: "18 de Julio",
            doorNumber: "1234",
            apartment: "Apto 101",
            items: items);

        _orderRepoMock.Verify(r => r.Add(It.Is<Order>(o =>
            o.ClientId == 1 &&
            o.Status == OrderStatus.Pending &&
            o.CreatedAt != default)), Times.Once);
    }

    [TestMethod]
    public void CreateOrder_EmptyItems_ThrowsArgumentExceptionWithExpectedMessage()
    {
        var items = new List<(string ProductCode, int Quantity)>();

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(
                clientId: 1,
                deliveryType: "express",
                street: "18 de Julio",
                doorNumber: "1234",
                apartment: "Apto 101",
                items: items));

        Assert.AreEqual("Order must have at least one product.", ex.Message);
    }
}

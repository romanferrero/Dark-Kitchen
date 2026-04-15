using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IUserRepository> _userRepoMock = null!;
    private Mock<IShippingCostCalculator> _shippingCalcMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _shippingCalcMock = new Mock<IShippingCostCalculator>(MockBehavior.Strict);
        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _promotionRepoMock.Object,
            _userRepoMock.Object,
            _shippingCalcMock.Object);
    }

    private void SetupValidClient(int clientId = 1)
    {
        var client = new User
        {
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass@1Ab!xyz",
            Role = UserRole.Client,
        };

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { client });
    }

    private void SetupExpressShipping()
    {
        _shippingCalcMock
            .Setup(c => c.Calculate("express"))
            .Returns(100m);
    }

    private void SetupStandardShipping()
    {
        _shippingCalcMock
            .Setup(c => c.Calculate("24hs"))
            .Returns(50m);
    }

    private Product CreateActiveProduct(string code, string name, decimal price)
    {
        var product = Product.Create(
            code: code,
            name: name,
            description: "Descripcion del producto de prueba",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/test.jpg",
            active: true);
        product.Price = price;
        return product;
    }

    [TestMethod]
    public void CreateOrder_WithPromotion_AppliesHighestDiscount()
    {
        SetupValidClient();
        SetupExpressShipping();

        var product = CreateActiveProduct("BURG01", "Hamburguesa clasica", 1000m);

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

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        var result = _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items);

        Assert.AreEqual(800m, result.Subtotal);
    }

    [TestMethod]
    public void CreateOrder_ExpressDelivery_ReturnsExactShippingAndTotal()
    {
        SetupValidClient();
        SetupExpressShipping();

        var product = CreateActiveProduct("BURG01", "Hamburguesa clasica", 200m);

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 2) };

        var result = _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items);

        Assert.AreEqual(400m, result.Subtotal);
        Assert.AreEqual(100m, result.ShippingCost);
        Assert.AreEqual(610m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_StandardDelivery_UsesStandardShippingCost()
    {
        SetupValidClient();
        SetupStandardShipping();

        var product = CreateActiveProduct("BURG01", "Hamburguesa clasica", 200m);

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 2) };

        var result = _orderService.CreateOrder(1, "24hs", "18 de Julio", "1234", "Apto 101", items);

        Assert.AreEqual(400m, result.Subtotal);
        Assert.AreEqual(50m, result.ShippingCost);
        Assert.AreEqual(549m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_ValidData_PersistsPendingOrder()
    {
        SetupValidClient();
        SetupExpressShipping();

        var product = CreateActiveProduct("BURG01", "Hamburguesa clasica", 200m);

        _productRepoMock
            .Setup(r => r.GetByCode("BURG01"))
            .Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items);

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
            _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items));

        Assert.AreEqual("Order must have at least one product.", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_WithMultipleProducts_ReturnsAccumulatedSubtotal()
    {
        SetupValidClient();
        SetupExpressShipping();

        var burger = CreateActiveProduct("BURG01", "Hamburguesa clasica", 200m);
        var pizza = CreateActiveProduct("PIZZA01", "Pizza muzzarella", 300m);

        _productRepoMock.Setup(r => r.GetByCode("BURG01")).Returns(burger);
        _productRepoMock.Setup(r => r.GetByCode("PIZZA01")).Returns(pizza);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);
        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "PIZZA01"))
            .Returns([]);

        _orderRepoMock.Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)>
        {
            ("BURG01", 2),
            ("PIZZA01", 1),
        };

        var result = _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items);

        Assert.AreEqual(700m, result.Subtotal);
        Assert.AreEqual(100m, result.ShippingCost);
        Assert.AreEqual(976m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_InactiveProduct_ThrowsArgumentExceptionWithExpectedMessage()
    {
        SetupValidClient();

        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg",
            active: false);
        product.Price = 200m;

        _productRepoMock.Setup(r => r.GetByCode("BURG01")).Returns(product);

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items));

        Assert.AreEqual("Product 'BURG01' is inactive and cannot be ordered.", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_ProductNotFound_ThrowsKeyNotFoundExceptionWithExpectedMessage()
    {
        SetupValidClient();

        _productRepoMock.Setup(r => r.GetByCode("NOEXIST")).Returns((Product?)null);

        var items = new List<(string ProductCode, int Quantity)> { ("NOEXIST", 1) };

        var ex = Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items));

        Assert.AreEqual("Product 'NOEXIST' not found.", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_ClientNotFound_ThrowsKeyNotFoundException()
    {
        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User>());

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        var ex = Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.CreateOrder(999, "express", "18 de Julio", "1234", "Apto 101", items));

        Assert.AreEqual("Client with id '999' not found.", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_UserIsNotClient_ThrowsArgumentException()
    {
        var admin = new User
        {
            FirstName = "Admin",
            LastName = "Usuario",
            Email = "admin@test.com",
            Phone = "099999999",
            Password = "AdminPass@1Ab!xyz",
            Role = UserRole.Admin,
        };

        _userRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>())).Returns(new List<User> { admin });

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items));

        Assert.AreEqual("Only clients can place orders.", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_InvalidDeliveryType_ThrowsArgumentException()
    {
        SetupValidClient();

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(1, "drone", "18 de Julio", "1234", "Apto 101", items));

        Assert.AreEqual("Delivery type 'drone' is not supported.", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_WithPromotion_StoresPromotionInfoInOrderItem()
    {
        SetupValidClient();
        SetupExpressShipping();

        var product = CreateActiveProduct("BURG01", "Hamburguesa clasica", 1000m);

        _productRepoMock.Setup(r => r.GetByCode("BURG01")).Returns(product);

        var promo = Promotion.Create("Black Friday", 20,
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([promo]);

        _orderRepoMock.Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items);

        _orderRepoMock.Verify(r => r.Add(It.Is<Order>(o =>
            o.Items[0].PromotionName == "Black Friday" &&
            o.Items[0].DiscountPercentage == 20 &&
            o.Items[0].OriginalPrice == 1000m &&
            o.Items[0].UnitPrice == 800m)), Times.Once);
    }

    [TestMethod]
    public void CreateOrder_WithoutPromotion_OrderItemHasNullPromotionFields()
    {
        SetupValidClient();
        SetupExpressShipping();

        var product = CreateActiveProduct("BURG01", "Hamburguesa clasica", 200m);

        _productRepoMock.Setup(r => r.GetByCode("BURG01")).Returns(product);

        _promotionRepoMock
            .Setup(r => r.GetFiltered(It.IsAny<DateOnly>(), null, "BURG01"))
            .Returns([]);

        _orderRepoMock.Setup(r => r.Add(It.IsAny<Order>()));

        var items = new List<(string ProductCode, int Quantity)> { ("BURG01", 1) };

        _orderService.CreateOrder(1, "express", "18 de Julio", "1234", "Apto 101", items);

        _orderRepoMock.Verify(r => r.Add(It.Is<Order>(o =>
            o.Items[0].PromotionName == null &&
            o.Items[0].DiscountPercentage == null &&
            o.Items[0].OriginalPrice == 200m &&
            o.Items[0].UnitPrice == 200m)), Times.Once);
    }
}

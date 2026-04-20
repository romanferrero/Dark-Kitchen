using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Discounts;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.IDiscounts;
using DarkKitchen.IBusinessLogic.IShippingCost;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IUserRepository> _userRepoMock = null!;
    private Mock<IShippingCostCalculatorFactory> _shippingFactoryMock = null!;
    private Mock<IShippingCostCalculator> _shippingCalcMock = null!;
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IDiscountCalculator> _discountCalculatorMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _shippingFactoryMock = new Mock<IShippingCostCalculatorFactory>(MockBehavior.Strict);
        _shippingCalcMock = new Mock<IShippingCostCalculator>(MockBehavior.Strict);
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _discountCalculatorMock = new Mock<IDiscountCalculator>(MockBehavior.Strict);

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _userRepoMock.Object,
            _shippingFactoryMock.Object,
            _promotionRepoMock.Object,
            _discountCalculatorMock.Object);
    }

    private Product BuildValidProduct()
    {
        var product = Product.Create(
            "PROD01",
            "Producto válido",
            "Descripción válida con suficiente longitud",
            "LineA",
            "CategoryA",
            "imagen1.jpg|100",
            true);

        product.Price = 100m;

        return product;
    }

    private Order BuildValidOrder()
    {
        return Order.Create(
            1,
            DeliveryType.Express,
            Address.Create("Calle", "123", "1"),
            [BuildValidProduct()],
            1,
            100,
            10m,
            2m,
            12m);
    }

    private static User MakeUser(int id = 1) => new()
    {
        Id = id,
        FirstName = "Juan",
        LastName = "Garcia",
        Email = "juan@gmail.com",
        Phone = "099123456",
        Password = "ValidPass1!extra",
        Role = UserRole.Client
    };

    private static Product MakeProduct(string code = "PROD-001", bool active = true, decimal price = 100m)
    {
        var product = Product.Create(
            code,
            "Producto de prueba uno",
            "Descripcion valida del producto de prueba numero uno",
            "LineA",
            "CategoryA",
            "image1.jpg|10",
            active);
        product.Price = price;
        return product;
    }

    private void SetupMocks(
        List<User> users,
        List<Product> products,
        List<Promotion>? promotions = null,
        decimal shippingCost = 50.0m)
    {
        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(users);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(products);

        _shippingFactoryMock
            .Setup(f => f.GetCalculator(DeliveryType.Express))
            .Returns(_shippingCalcMock.Object);

        _shippingCalcMock
            .Setup(c => c.GetCost())
            .Returns(shippingCost);

        _promotionRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotions ?? []);

        _discountCalculatorMock
            .Setup(c => c.CalculatePrice(It.IsAny<Product>(), It.IsAny<List<Promotion>>()))
            .Returns((Product p, List<Promotion> promos) => new BestDiscountCalculator().CalculatePrice(p, promos));

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));
    }

    [TestMethod]
    public void CreateOrder_WithInactiveProduct_ThrowsArgumentException()
    {
        var user = MakeUser();
        var inactiveProduct = MakeProduct(active: false);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns([inactiveProduct]);

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(
                user.Id, DeliveryType.Express.ToString(),
                "18 de Julio", "1234", "3B",
                ["PROD-001"]));
    }

    [TestMethod]
    public void CreateOrder_WhenUserListIsEmpty_ThrowsArgumentException()
    {
        var product = MakeProduct();
        SetupMocks(users: [], products: [product]);

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(
                999,
                DeliveryType.Express.ToString(),
                "18 de Julio",
                "1234",
                "3B",
                ["PROD-001"]));

        Assert.AreEqual("User not found", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_AppliesIVAToTotal()
    {
        var user = MakeUser();
        var product = MakeProduct();
        SetupMocks(users: [user], products: [product]);

        var result = _orderService.CreateOrder(
            user.Id, DeliveryType.Express.ToString(),
            "Av. 18 de Julio", "1234", "3B",
            ["PROD-001"]);

        // total = (subtotal + shipping) * 1.22 = (100 + 50) * 1.22 = 183
        Assert.AreEqual(183m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_Valid()
    {
        var user = MakeUser();
        var product = MakeProduct();
        SetupMocks(users: [user], products: [product]);

        var result = _orderService.CreateOrder(
            user.Id,
            DeliveryType.Express.ToString(),
            "Av. 18 de Julio",
            "1234",
            "3B",
            ["PROD-001"]);

        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.ClientId);
        Assert.AreEqual(100m, result.Subtotal);
        Assert.AreEqual(50m, result.ShippingCost);

        // total = (subtotal + shipping) * 1.22 = (100 + 50) * 1.22 = 183
        Assert.AreEqual(183m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_WithActivePromotion_AppliesDiscountBeforeIVA()
    {
        var user = MakeUser();
        var product = MakeProduct();

        var promotion = Promotion.Create("10% off", 10, DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promotion.AddProduct(product);

        SetupMocks(users: [user], products: [product], promotions: [promotion]);

        var result = _orderService.CreateOrder(
            user.Id, DeliveryType.Express.ToString(),
            "18 de Julio", "1234", "3B",
            ["PROD-001"]);

        // product 100 with 10% off = 90, subtotal = 90
        // total = (90 + 50) * 1.22 = 170.8
        Assert.AreEqual(90m, result.Subtotal);
        Assert.AreEqual(170.8m, result.Total);
    }

    [TestMethod]
    public void UpdateStatus_ValidTransition_UpdatesStatusAndReturnsDTO()
    {
        var order = BuildValidOrder();

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([order]);

        _orderRepoMock
            .Setup(r => r.Update(order));

        var dto = new UpdateOrderStatusEntryDTO("Prepared");

        var result = _orderService.UpdateStatus(order.OrderId, dto);

        Assert.AreEqual(OrderStatus.Prepared, order.OrderStatus);

        Assert.IsNotNull(result);
        Assert.AreEqual("Prepared", result.Status);
        Assert.IsTrue(result.UpdatedAt <= DateTime.Now);
        Assert.IsTrue(result.UpdatedAt > DateTime.Now.AddSeconds(-5));

        _orderRepoMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void CreateOrder_UserNotFound_ThrowsArgumentException()
    {
        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(99, "Express", "Calle", "123", "1A", ["PROD01"]));
    }

    [TestMethod]
    public void CreateOrder_InvalidDeliveryType_ThrowsException()
    {
        var user = new User { Id = 1, Role = UserRole.Client };

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns([BuildValidProduct()]);

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(1, "TipoInvalido", "Calle", "123", "1A", ["PROD01"]));
    }

    [TestMethod]
    public void UpdateStatus_OrderNotFound_ThrowsKeyNotFoundException()
    {
        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([]);

        var dto = new UpdateOrderStatusEntryDTO("Prepared");

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.UpdateStatus(999, dto));
    }

    [TestMethod]
    public void UpdateStatus_InvalidStatus_ThrowsArgumentException()
    {
        var order = BuildValidOrder();

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([order]);

        var dto = new UpdateOrderStatusEntryDTO("StatusInvalido");

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.UpdateStatus(order.OrderId, dto));
    }

    [TestMethod]
    public void GetClientOrders_ReturnsOrderSummariesForClient()
    {
        var clientId = 1;

        var user = new User
        {
            Id = clientId,
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass1!extra",
            Role = UserRole.Client
        };

        var product = Product.Create(
            "PROD-001",
            "Producto de prueba uno",
            "Descripcion valida del producto numero uno",
            "LineA",
            "CategoryA",
            "img.jpg|10",
            true);
        product.Price = 100m;

        var address = Address.Create("18 de Julio", "1234", "3B");

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 7,
            subtotal: 100.0m,
            shippingCost: 20.0m,
            totalCost: 146.4m);

        _orderRepoMock
            .Setup(r => r.GetClientOrders(clientId, null, null, null))
            .Returns([order]);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        var result = _orderService.GetClientOrders(clientId, null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(7, result[0].OrderNumber);
        Assert.AreEqual(clientId, result[0].ClientId);
        Assert.AreEqual("Juan Garcia", result[0].ClientFullName);
        Assert.AreEqual("Pending", result[0].Status);
        Assert.AreEqual(1, result[0].ProductCount);
    }

    [TestMethod]
    public void GetClientOrders_WithLowercaseStatus_ParsesWithoutThrowing()
    {
        var clientId = 1;

        var product = Product.Create(
            "PROD-001",
            "Producto de prueba uno",
            "Descripcion valida del producto numero uno",
            "LineA",
            "CategoryA",
            "img.jpg|10",
            true);

        var address = Address.Create("18 de Julio", "1234", "3B");
        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 70,
            subtotal: 100.0m,
            shippingCost: 20.0m,
            totalCost: 146.4m);
        order.OrderStatus = OrderStatus.Prepared;

        _orderRepoMock
            .Setup(r => r.GetClientOrders(clientId, null, null, OrderStatus.Prepared))
            .Returns([order]);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);

        var result = _orderService.GetClientOrders(clientId, null, null, "prepared");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Prepared", result[0].Status);
    }

    [TestMethod]
    public void GetDispatcherOrders_ValidDateRange_ReturnsOrders()
    {
        var from = DateTime.Today.AddDays(-7);
        var to = DateTime.Today;
        var clientId = 2;

        var user = new User
        {
            Id = clientId,
            FirstName = "Maria",
            LastName = "Lopez",
            Email = "maria@test.com",
            Phone = "099000000",
            Password = "ValidPass1!extra",
            Role = UserRole.Client
        };

        var product = Product.Create(
            "PROD-002",
            "Producto de prueba dos",
            "Descripcion valida del producto numero dos",
            "LineB",
            "CategoryB",
            "img.jpg|10",
            true);
        product.Price = 200m;

        var address = Address.Create("Bv. Artigas", "9999", string.Empty);

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.TwentyFourHours,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 3,
            subtotal: 200.0m,
            shippingCost: 10.0m,
            totalCost: 256.4m);

        _orderRepoMock
            .Setup(r => r.GetOrdersByDateRange(from, to, null, null))
            .Returns([order]);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        var result = _orderService.GetDispatcherOrders(from, to, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(3, result[0].OrderNumber);
        Assert.AreEqual(clientId, result[0].ClientId);
        Assert.AreEqual("Maria Lopez", result[0].ClientFullName);
        Assert.AreEqual("Pending", result[0].Status);
    }

    [TestMethod]
    public void GetDispatcherOrders_WithLowercaseStatus_ParsesWithoutThrowing()
    {
        var from = DateTime.Today.AddDays(-7);
        var to = DateTime.Today;
        var clientId = 2;

        var user = new User
        {
            Id = clientId,
            FirstName = "Maria",
            LastName = "Lopez",
            Email = "maria@test.com",
            Phone = "099000000",
            Password = "ValidPass1!extra",
            Role = UserRole.Client
        };

        var product = Product.Create(
            "PROD-002",
            "Producto de prueba dos",
            "Descripcion valida del producto numero dos",
            "LineB",
            "CategoryB",
            "img.jpg|10",
            true);

        var address = Address.Create("Bv. Artigas", "9999", string.Empty);

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.TwentyFourHours,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 5,
            subtotal: 200.0m,
            shippingCost: 10.0m,
            totalCost: 256.4m);
        order.OrderStatus = OrderStatus.Prepared;

        _orderRepoMock
            .Setup(r => r.GetOrdersByDateRange(from, to, null, OrderStatus.Prepared))
            .Returns([order]);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        var result = _orderService.GetDispatcherOrders(from, to, null, "prepared");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Prepared", result[0].Status);
    }

    [TestMethod]
    public void GetOrderById_ExistingOrder_ReturnsOrderDetail()
    {
        var clientId = 1;

        var user = new User
        {
            Id = clientId,
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass1!extra",
            Role = UserRole.Client
        };

        var product = Product.Create(
            "PROD-001",
            "Producto de prueba uno",
            "Descripcion valida del producto numero uno",
            "LineA",
            "CategoryA",
            "img.jpg|10",
            true);
        product.Price = 100m;

        var address = Address.Create("18 de Julio", "1234", "3B");

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 99,
            subtotal: 100.0m,
            shippingCost: 20.0m,
            totalCost: 146.4m);

        _orderRepoMock.Setup(r => r.GetOrderById(99)).Returns(order);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        _promotionRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([]);

        var result = _orderService.GetOrderById(99);

        Assert.AreEqual(99, result.OrderNumber);
        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual("Juan Garcia", result.ClientFullName);
        Assert.AreEqual("Pending", result.Status);
        Assert.AreEqual(1, result.Products.Count);
        Assert.AreEqual("PROD-001", result.Products[0].Code);
    }

    [TestMethod]
    public void GetOrderById_NonExistentOrder_ThrowsKeyNotFoundException()
    {
        _orderRepoMock.Setup(r => r.GetOrderById(999)).Returns((Order?)null);

        Assert.ThrowsException<KeyNotFoundException>(() => _orderService.GetOrderById(999));
    }

    [TestMethod]
    public void GetOrderById_WhenClientUserIsMissing_ReturnsUnknownClientName()
    {
        var clientId = 1;

        var product = Product.Create(
            "PROD-001",
            "Producto de prueba uno",
            "Descripcion valida del producto numero uno",
            "LineA",
            "CategoryA",
            "img.jpg|10",
            true);
        product.Price = 100m;

        var address = Address.Create("18 de Julio", "1234", "3B");

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 100,
            subtotal: 100.0m,
            shippingCost: 20.0m,
            totalCost: 146.4m);

        _orderRepoMock.Setup(r => r.GetOrderById(100)).Returns(order);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);

        _promotionRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([]);

        var result = _orderService.GetOrderById(100);

        Assert.AreEqual("Unknown client", result.ClientFullName);
    }

    [TestMethod]
    public void GetOrderById_WhenPromotionsHaveSameDiscount_UsesAlphabeticalNameAsTieBreaker()
    {
        var clientId = 1;

        var user = new User
        {
            Id = clientId,
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@test.com",
            Phone = "099123456",
            Password = "ValidPass1!extra",
            Role = UserRole.Client
        };

        var product = Product.Create(
            "PROD-001",
            "Producto de prueba uno",
            "Descripcion valida del producto numero uno",
            "LineA",
            "CategoryA",
            "img.jpg|10",
            true);
        product.Price = 100m;

        var address = Address.Create("18 de Julio", "1234", "3B");

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 101,
            subtotal: 100.0m,
            shippingCost: 20.0m,
            totalCost: 146.4m);

        var promoB = Promotion.Create("B Promo", 20, DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promoB.AddProduct(product);

        var promoA = Promotion.Create("A Promo", 20, DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promoA.AddProduct(product);

        _orderRepoMock.Setup(r => r.GetOrderById(101)).Returns(order);

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        _promotionRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([promoB, promoA]);

        var result = _orderService.GetOrderById(101);

        Assert.AreEqual("A Promo", result.Products[0].PromotionName);
        Assert.AreEqual(20, result.Products[0].DiscountPercentage);
    }
}

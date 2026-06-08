using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Discounts;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.DTOs.Entry.OrderDTOs;
using DarkKitchen.IBusinessLogic.IDiscounts;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IRepository<User>> _userRepoMock = null!;
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private Mock<IDiscountCalculator> _discountCalculatorMock = null!;
    private Mock<IRepository<DeliveryType>> _deliveryTypeRepoMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _userRepoMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _discountCalculatorMock = new Mock<IDiscountCalculator>(MockBehavior.Strict);
        _deliveryTypeRepoMock = new Mock<IRepository<DeliveryType>>(MockBehavior.Strict);

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _userRepoMock.Object,
            _promotionRepoMock.Object,
            _discountCalculatorMock.Object,
            _deliveryTypeRepoMock.Object);
    }

    private static User CreateUser(int id = 1) => new()
    {
        Id = id,
        FirstName = "Juan",
        LastName = "Garcia",
        Email = "juan@gmail.com",
        Phone = "099123456",
        Password = "ValidPass1!extra",
        Role = UserRole.Client
    };

    private static Product CreateProduct(
        string code = "PROD-001",
        decimal price = 100m,
        bool active = true,
        string name = "Producto de prueba valido",
        string description = "Descripcion valida suficientemente larga para dominio",
        string line = "LineA",
        string category = "CategoryA",
        string images = "img.jpg|100")
    {
        return Product.Create(new CreateProductParams(
            code,
            name,
            price,
            description,
            line,
            category,
            images,
            active));
    }

    private static List<OrderProduct> ToOrderProducts(Product product, int quantity = 1)
    {
        return [new OrderProduct { ProductId = product.Id, Product = product, Quantity = quantity }];
    }

    private static List<OrderProductEntryDto> CreateProducts(string code, int quantity = 1)
    {
        return [new OrderProductEntryDto(code, quantity)];
    }

    private void SetupMocks(
        List<User> users,
        List<Product> products,
        List<Promotion>? promotions = null)
    {
        _userRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(users.Count > 0);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(products);

        _promotionRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotions ?? []);

        _discountCalculatorMock
            .Setup(c => c.CalculatePrice(It.IsAny<Product>(), It.IsAny<List<Promotion>>()))
            .Returns((Product p, List<Promotion> promos) =>
                new BestDiscountCalculator().CalculatePrice(p, promos));

        _orderRepoMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(false);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        _deliveryTypeRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns(DeliveryType.Create("Express", 250m));
    }

    [TestMethod]
    public void CreateOrder_UserNotFound_Throws()
    {
        _userRepoMock.Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);

        var dto = new CreateOrderEntryDto(1, "Express", "Calle", "123", "A", CreateProducts("PROD-001"));

        Assert.ThrowsException<ArgumentException>(() => _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void CreateOrder_InactiveProduct_Throws()
    {
        var user = CreateUser();
        var product = CreateProduct(active: false);

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", CreateProducts("PROD-001"));

        Assert.ThrowsException<ArgumentException>(() => _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void CreateOrder_Valid()
    {
        var user = CreateUser();
        var product = CreateProduct();

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", CreateProducts("PROD-001"));

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(100m, result.Subtotal);
        Assert.AreEqual(250m, result.ShippingCost);
        Assert.AreEqual((100m + 250m) * 1.22m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_WithPromotion()
    {
        var user = CreateUser();
        var product = CreateProduct();

        var promo = Promotion.Create(
            "Promo",
            10,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));

        promo.AddProduct(product);

        SetupMocks([user], [product], [promo]);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", CreateProducts("PROD-001"));

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(90m, result.Subtotal);
        Assert.AreEqual((90m + 250m) * 1.22m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_MultipleQuantity_CalculatesSubtotalCorrectly()
    {
        var user = CreateUser();
        var product = CreateProduct(price: 200m);

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(
            user.Id, "Express", "Calle", "123", "A",
            [new OrderProductEntryDto("PROD-001", 3)]);

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(600m, result.Subtotal);
        Assert.AreEqual((600m + 250m) * 1.22m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_WithPromotionAndQuantity_AppliesDiscountPerUnit()
    {
        var user = CreateUser();
        var product = CreateProduct(price: 100m);

        var promo = Promotion.Create(
            "Promo", 20,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promo.AddProduct(product);

        SetupMocks([user], [product], [promo]);

        var dto = new CreateOrderEntryDto(
            user.Id, "Express", "Calle", "123", "A",
            [new OrderProductEntryDto("PROD-001", 2)]);

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(160m, result.Subtotal);
        Assert.AreEqual((160m + 250m) * 1.22m, result.Total);
    }

    [TestMethod]
    public void UpdateStatus_NotFound_Throws()
    {
        _orderRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns((Order?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.UpdateStatus(1, new("Prepared")));
    }

    [TestMethod]
    public void GetOrderById_NotFound_Throws()
    {
        _orderRepoMock.Setup(r => r.GetOrderById(1)).Returns((Order?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.GetOrderById(1));
    }

    [TestMethod]
    public void GetOrderById_Valid()
    {
        var user = CreateUser();
        var product = CreateProduct();

        var order = Order.Create(new CreateOrderParams(
            "Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product),
            user.Id,
            10,
            100,
            20,
            146.4m));
        order.OrderId = 55;

        _orderRepoMock.Setup(r => r.GetOrderById(55)).Returns(order);

        _userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(user);

        _promotionRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([]);

        var result = _orderService.GetOrderById(55);

        Assert.AreEqual(55, result.OrderId);
        Assert.AreEqual(10, result.OrderNumber);
        Assert.AreEqual("Juan Garcia", result.ClientFullName);
        Assert.AreEqual(1, result.Products.Count);
        Assert.AreEqual(1, result.Products[0].Quantity);
    }

    [TestMethod]
    public void GetClientOrders_Valid()
    {
        var user = CreateUser();
        var product = CreateProduct();

        var order = Order.Create(new CreateOrderParams(
            "Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product),
            user.Id,
            5,
            100,
            20,
            146.4m));
        order.OrderId = 12;

        _orderRepoMock.Setup(r => r.GetClientOrders(user.Id, null, null, null))
            .Returns([order]);

        _userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(user);

        var result = _orderService.GetClientOrders(user.Id, null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(12, result[0].OrderId);
        Assert.AreEqual("Juan Garcia", result[0].ClientFullName);
    }

    [TestMethod]
    public void GetDispatcherOrders_Valid()
    {
        var user = CreateUser(2);
        var product = CreateProduct();

        var order = Order.Create(new CreateOrderParams(
            "Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product),
            user.Id,
            7,
            100,
            20,
            146.4m));
        order.OrderId = 31;

        _orderRepoMock.Setup(r =>
                r.GetOrdersByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, null))
            .Returns([order]);

        _userRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        var result = _orderService.GetDispatcherOrders(
            DateTime.Today.AddDays(-1),
            DateTime.Today,
            null,
            null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(31, result[0].OrderId);
        Assert.AreEqual("Juan Garcia", result[0].ClientFullName);
    }

    [TestMethod]
    public void UpdateStatus_Valid_UpdatesAndReturns()
    {
        var product = CreateProduct();
        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), 1, 10, 100, 20, 146.4m));

        _orderRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(order);
        _orderRepoMock.Setup(r => r.Update(It.IsAny<Order>()));

        var result = _orderService.UpdateStatus(1, new("Prepared"));

        Assert.AreEqual("Prepared", result.Status);
    }

    [TestMethod]
    public void UpdateStatus_InvalidAction_Throws()
    {
        var product = CreateProduct();
        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), 1, 10, 100, 20, 146.4m));

        _orderRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(order);

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.UpdateStatus(1, new("INVALID")));
    }

    [TestMethod]
    public void CreateOrder_InvalidDeliveryType_Throws()
    {
        var user = CreateUser();
        var product = CreateProduct();
        SetupMocks([user], [product]);

        _deliveryTypeRepoMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<DeliveryType, bool>>>()))
            .Returns((DeliveryType?)null);

        var dto = new CreateOrderEntryDto(user.Id, "INVALID", "Calle", "123", "A", CreateProducts("PROD-001"));

        Assert.ThrowsException<ArgumentException>(() => _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void GetClientOrders_WithStatusFilter_ReturnsFiltered()
    {
        var user = CreateUser();
        var product = CreateProduct();
        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), user.Id, 5, 100, 20, 146.4m));

        _orderRepoMock.Setup(r => r.GetClientOrders(user.Id, null, null, "Pending"))
            .Returns([order]);

        _userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(user);

        var result = _orderService.GetClientOrders(user.Id, null, null, "Pending");

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetClientOrders_InvalidStatus_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.GetClientOrders(1, null, null, "INVALID"));
    }

    [TestMethod]
    public void GetDispatcherOrders_WithStreetAndStatus_ReturnsFiltered()
    {
        var user = CreateUser(2);
        var product = CreateProduct();
        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), user.Id, 7, 100, 20, 146.4m));

        _orderRepoMock.Setup(r =>
                r.GetOrdersByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "Calle", "Pending"))
            .Returns([order]);

        _userRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        var result = _orderService.GetDispatcherOrders(
            DateTime.Today.AddDays(-1), DateTime.Today, "Calle", "Pending");

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetDispatcherOrders_InvalidStatus_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.GetDispatcherOrders(DateTime.Today, DateTime.Today, null, "INVALID"));
    }

    [TestMethod]
    public void GetOrderById_ClientNotFound_ReturnsUnknownClient()
    {
        var product = CreateProduct();
        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), 1, 10, 100, 20, 146.4m));

        _orderRepoMock.Setup(r => r.GetOrderById(10)).Returns(order);

        _userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns((User?)null);

        _promotionRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([]);

        var result = _orderService.GetOrderById(10);

        Assert.AreEqual("Unknown client", result.ClientFullName);
    }

    [TestMethod]
    public void GetOrderById_ProductWithPromotion_ReturnsPromotionDetails()
    {
        var user = CreateUser();
        var product = CreateProduct();

        var promo = Promotion.Create("Oferta", 15,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promo.AddProduct(product);

        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), user.Id, 10, 100, 20, 146.4m));

        _orderRepoMock.Setup(r => r.GetOrderById(10)).Returns(order);
        _userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(user);
        _promotionRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([promo]);

        var result = _orderService.GetOrderById(10);

        Assert.AreEqual("Oferta", result.Products[0].PromotionName);
        Assert.AreEqual(15, result.Products[0].DiscountPercentage);
    }

    [TestMethod]
    public void GetOrderById_MultiplePromotions_ReturnsBestDiscount()
    {
        var user = CreateUser();
        var product = CreateProduct();

        var promo10 = Promotion.Create("Promo10", 10,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promo10.AddProduct(product);

        var promo25 = Promotion.Create("Promo25", 25,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));
        promo25.AddProduct(product);

        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), user.Id, 10, 100, 20, 146.4m));

        _orderRepoMock.Setup(r => r.GetOrderById(10)).Returns(order);
        _userRepoMock.Setup(r => r.Get(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(user);
        _promotionRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns([promo10, promo25]);

        var result = _orderService.GetOrderById(10);

        Assert.AreEqual("Promo25", result.Products[0].PromotionName);
        Assert.AreEqual(25, result.Products[0].DiscountPercentage);
    }

    [TestMethod]
    public void CreateOrder_ZeroQuantity_Throws()
    {
        var user = CreateUser();
        var product = CreateProduct();

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(
            user.Id, "Express", "Calle", "123", "A",
            [new OrderProductEntryDto("PROD-001", 0)]);

        Assert.ThrowsException<ArgumentException>(() => _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void GetDispatcherOrders_ClientNotFound_ReturnsUnknownClient()
    {
        var product = CreateProduct();
        var order = Order.Create(new CreateOrderParams("Express",
            Address.Create("Calle", "123", "A"),
            ToOrderProducts(product), 99, 7, 100, 20, 146.4m));

        _orderRepoMock.Setup(r =>
                r.GetOrdersByDateRange(It.IsAny<DateTime>(), It.IsAny<DateTime>(), null, null))
            .Returns([order]);

        _userRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);

        var result = _orderService.GetDispatcherOrders(DateTime.Today, DateTime.Today, null, null);

        Assert.AreEqual("Unknown client", result[0].ClientFullName);
    }
}

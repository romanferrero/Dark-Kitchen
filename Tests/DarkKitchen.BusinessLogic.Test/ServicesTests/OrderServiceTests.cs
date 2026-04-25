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
        _orderRepoMock = new Mock<IOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _shippingFactoryMock = new Mock<IShippingCostCalculatorFactory>();
        _shippingCalcMock = new Mock<IShippingCostCalculator>();
        _promotionRepoMock = new Mock<IPromotionRepository>();
        _discountCalculatorMock = new Mock<IDiscountCalculator>();

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _userRepoMock.Object,
            _shippingFactoryMock.Object,
            _promotionRepoMock.Object,
            _discountCalculatorMock.Object);
    }

    // =========================
    // FIX PRODUCT CREATION
    // =========================
    private static Product CreateProduct(
        string code = "PROD001",
        decimal price = 100m,
        bool active = true,
        string name = "Producto valido nombre",
        string description = "Descripcion suficientemente larga para dominio",
        string line = "LineA",
        string category = "CategoryA",
        string images = "img.jpg|100")
    {
        return Product.Create(
            code,
            name,
            price,
            description,
            line,
            category,
            images,
            active);
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

    private void SetupMocks(
        List<User> users,
        List<Product> products,
        List<Promotion>? promotions = null,
        decimal shippingCost = 50m)
    {
        _userRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(users);

        _productRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(products);

        _shippingFactoryMock.Setup(f => f.GetCalculator(It.IsAny<DeliveryType>()))
            .Returns(_shippingCalcMock.Object);

        _shippingCalcMock.Setup(c => c.GetCost())
            .Returns(shippingCost);

        _promotionRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Promotion, bool>>>()))
            .Returns(promotions ?? []);

        _discountCalculatorMock.Setup(c => c.CalculatePrice(It.IsAny<Product>(), It.IsAny<List<Promotion>>()))
            .Returns((Product p, List<Promotion> promos) =>
                new BestDiscountCalculator().CalculatePrice(p, promos));

        _orderRepoMock.Setup(r => r.Add(It.IsAny<Order>()));
    }

    [TestMethod]
    public void CreateOrder_UserNotFound_Throws()
    {
        _userRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);

        var dto = new CreateOrderEntryDto(1, "Express", "Calle", "123", "A", ["PROD001"]);

        Assert.ThrowsException<ArgumentException>(() => _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void CreateOrder_InactiveProduct_Throws()
    {
        var user = CreateUser();
        var product = CreateProduct(active: false);

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", ["PROD001"]);

        Assert.ThrowsException<ArgumentException>(() => _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void CreateOrder_Valid()
    {
        var user = CreateUser();
        var product = CreateProduct(price: 100m);

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", ["PROD001"]);

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(100m, result.Subtotal);
        Assert.AreEqual(50m, result.ShippingCost);
        Assert.AreEqual(183m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_WithPromotion()
    {
        var user = CreateUser();
        var product = CreateProduct(price: 100m);

        var promo = Promotion.Create(
            "Promo",
            10,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today));

        promo.AddProduct(product);

        SetupMocks([user], [product], [promo]);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", ["PROD001"]);

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(90m, result.Subtotal);
        Assert.AreEqual(170.8m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_ProductNotFound_Throws()
    {
        var user = CreateUser();

        SetupMocks([user], []);

        var dto = new CreateOrderEntryDto(user.Id, "Express", "Calle", "123", "A", ["PROD001"]);

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void CreateOrder_MultipleProducts_CalculatesCorrectly()
    {
        var user = CreateUser();

        var p1 = CreateProduct(code: "PROD001", price: 100m);
        var p2 = CreateProduct(code: "PROD002", price: 200m);

        SetupMocks([user], [p1, p2]);

        var dto = new CreateOrderEntryDto(
            user.Id,
            "Express",
            "Calle",
            "123",
            "A",
            ["PROD001", "PROD002"]);

        var result = _orderService.CreateOrder(dto);

        Assert.AreEqual(300m, result.Subtotal);
        Assert.AreEqual(50m, result.ShippingCost);
        Assert.AreEqual(427m, result.Total);
    }

    [TestMethod]
    public void CreateOrder_InvalidDeliveryType_Throws()
    {
        var user = CreateUser();
        var product = CreateProduct();

        SetupMocks([user], [product]);

        var dto = new CreateOrderEntryDto(
            user.Id,
            "INVALIDO",
            "Calle",
            "123",
            "A",
            ["PROD001"]);

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.CreateOrder(dto));
    }

    [TestMethod]
    public void UpdateStatus_NotFound_Throws()
    {
        _orderRepoMock.Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([]);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.UpdateStatus(1, new("Prepared")));
    }

    [TestMethod]
    public void GetOrderById_NotFound_Throws()
    {
        _orderRepoMock.Setup(r => r.GetOrderById(1))
            .Returns((Order?)null);

        Assert.ThrowsException<KeyNotFoundException>(() =>
            _orderService.GetOrderById(1));
    }
}

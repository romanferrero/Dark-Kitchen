using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class GetOrderByIdServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IUserRepository> _userRepoMock = null!;
    private Mock<IShippingCostCalculatorFactory> _shippingFactoryMock = null!;
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

        Assert.ThrowsException<KeyNotFoundException>(
            () => _orderService.GetOrderById(999));
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

        var promoB = Promotion.Create("B Promo", 20, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today));
        promoB.AddProduct(product);

        var promoA = Promotion.Create("A Promo", 20, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today));
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

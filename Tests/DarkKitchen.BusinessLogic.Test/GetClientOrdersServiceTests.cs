using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class GetClientOrdersServiceTests
{
    private Mock<IOrderRepository> _orderRepoMock = null!;
    private Mock<IProductRepository> _productRepoMock = null!;
    private Mock<IUserRepository> _userRepoMock = null!;
    private Mock<IShippingCostCalculatorFactory> _shippingFactoryMock = null!;
    private Mock<IPromotionRepository> _promotionRepoMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _shippingFactoryMock = new Mock<IShippingCostCalculatorFactory>(MockBehavior.Strict);
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _userRepoMock.Object,
            _shippingFactoryMock.Object,
            _promotionRepoMock.Object);
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
            subtotal: 100.0,
            shippingCost: 20.0,
            totalCost: 146.4);

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
            subtotal: 100.0,
            shippingCost: 20.0,
            totalCost: 146.4);
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
}

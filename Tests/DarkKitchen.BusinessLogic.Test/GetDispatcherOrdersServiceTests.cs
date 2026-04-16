using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class GetDispatcherOrdersServiceTests
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

        var address = Address.Create("Bv. Artigas", "9999", null);

        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.TwentyFourHours,
            address: address,
            products: [product],
            clientId: clientId,
            orderNumber: 3,
            subtotal: 200.0,
            shippingCost: 10.0,
            totalCost: 256.4);

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
}

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
    private Mock<IUserRepository> _userRepoMock = null!;
    private Mock<IShippingCostCalculator> _shippingCalcMock = null!;
    private Mock<IOrderFactory> _orderFactoryMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _shippingCalcMock = new Mock<IShippingCostCalculator>(MockBehavior.Strict);
        _orderFactoryMock = new Mock<IOrderFactory>(MockBehavior.Strict);

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _userRepoMock.Object,
            _shippingCalcMock.Object,
            _orderFactoryMock.Object);
    }

    [TestMethod]
    public void OrderService_CreateOrder_Valid()
    {
        var clientId = 1;
        var street = "Av. 18 de Julio";
        var doorNumber = "1234";
        var apartment = "3B";

        var user = new User
        {
            Id = clientId,
            FirstName = "Juan",
            LastName = "Garcia",
            Email = "juan@gmail.com",
            Phone = "099123456",
            Password = "ValidPass1!extra",
            Role = UserRole.Client
        };

        var product1 = Product.Create(
            "PROD-001",
            "Producto de prueba uno",
            "Descripcion valida del producto de prueba numero uno",
            "LineA",
            "CategoryA",
            "image1.jpg|10",
            true);
        product1.Price = 100m;

        var items = new List<string> { "PROD-001" };

        var expectedSubtotal = 100m;
        var expectedShipping = 50m;
        var expectedTotal = 150m;

        var createdOrder = Order.Create(
            0,
            DeliveryType.Express,
            Address.Create(street, doorNumber, apartment),
            [product1],
            clientId,
            1,
            (double)expectedSubtotal,
            (double)expectedShipping,
            (double)expectedTotal);

        // IRepository<User>.GetAll(predicate) — se usa en ValidateClientExists
        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        _productRepoMock.Setup(r => r.GetByCode("PROD-001")).Returns(product1);
        _shippingCalcMock.Setup(c => c.GetCost()).Returns((double)expectedShipping);

        _orderFactoryMock
            .Setup(f => f.CreateOrder(
                It.IsAny<int>(),
                DeliveryType.Express,
                It.IsAny<Address>(),
                It.IsAny<List<Product>>(),
                clientId,
                It.IsAny<int>(),
                (double)expectedSubtotal,
                (double)expectedShipping,
                (double)expectedTotal))
            .Returns(createdOrder);

        _orderRepoMock.Setup(r => r.Add(It.IsAny<Order>()));

        var result = _orderService.CreateOrder(
            clientId,
            DeliveryType.Express.ToString(),
            street,
            doorNumber,
            apartment,
            items);

        Assert.IsNotNull(result);
        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual(expectedSubtotal, result.Subtotal);
        Assert.AreEqual(expectedShipping, result.ShippingCost);
        Assert.AreEqual(expectedTotal, result.Total);

        _userRepoMock.Verify(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        _productRepoMock.Verify(r => r.GetByCode("PROD-001"), Times.Once);
        _shippingCalcMock.Verify(c => c.GetCost(), Times.Once);
        _orderRepoMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void OrderService_CreateOrder_ClientNotFound_ThrowsKeyNotFoundException()
    {
        var clientId = 99;

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([]);

        _orderService.CreateOrder(
            clientId,
            DeliveryType.Express.ToString(),
            "Calle Falsa",
            "123",
            "A",
            ["PROD-001"]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void OrderService_CreateOrder_UserIsNotClient_ThrowsArgumentException()
    {
        var adminId = 2;

        var admin = new User
        {
            Id = adminId,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@dark.com",
            Phone = "099000000",
            Password = "AdminPass1!extra",
            Role = UserRole.Admin
        };

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([admin]);

        _orderService.CreateOrder(
            adminId,
            DeliveryType.Express.ToString(),
            "Calle Falsa",
            "123",
            "A",
            ["PROD-001"]);
    }
}

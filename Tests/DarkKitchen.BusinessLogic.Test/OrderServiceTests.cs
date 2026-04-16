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
    private Mock<IRepository<Order>> _orderRepoMock = null!;
    private Mock<IRepository<Product>> _productRepoMock = null!;
    private Mock<IRepository<User>> _userRepoMock = null!;
    private Mock<IShippingCostCalculatorFactory> _shippingFactoryMock = null!;
    private Mock<IShippingCostCalculator> _shippingCalcMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IRepository<Order>>(MockBehavior.Strict);
        _productRepoMock = new Mock<IRepository<Product>>(MockBehavior.Strict);
        _userRepoMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _shippingFactoryMock = new Mock<IShippingCostCalculatorFactory>(MockBehavior.Strict);
        _shippingCalcMock = new Mock<IShippingCostCalculator>(MockBehavior.Strict);

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _userRepoMock.Object,
            _shippingFactoryMock.Object);
    }

    [TestMethod]
    public void CreateOrder_Valid()
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

        var expectedShipping = 50.0;

        _userRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns([user]);

        _productRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns([product1]);

        _shippingFactoryMock
            .Setup(f => f.GetCalculator(DeliveryType.Express))
            .Returns(_shippingCalcMock.Object);

        _shippingCalcMock
            .Setup(c => c.GetCost())
            .Returns(expectedShipping);

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        var result = _orderService.CreateOrder(
            clientId,
            DeliveryType.Express.ToString(),
            street,
            doorNumber,
            apartment,
            items);

        Assert.IsNotNull(result);
        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual(100m, result.Subtotal);
        Assert.AreEqual((decimal)expectedShipping, result.ShippingCost);
        Assert.AreEqual(150m, result.Total);
    }
}

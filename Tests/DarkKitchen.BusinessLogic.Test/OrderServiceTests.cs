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
            10,
            2,
            12);
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

    [TestMethod]
    public void UpdateStatus_ValidTransition_UpdatesStatusAndReturnsDTO()
    {
        var order = BuildValidOrder();

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([order]);

        _orderRepoMock
            .Setup(r => r.Update(order));

        var result = _orderService.UpdateStatus(order.OrderId);

        Assert.AreEqual(OrderStatus.Prepared, order.OrderStatus);

        Assert.IsNotNull(result);
        Assert.AreEqual("Prepared", result.Status);
        Assert.IsTrue(result.UpdatedAt <= DateTime.Now);
        Assert.IsTrue(result.UpdatedAt > DateTime.Now.AddSeconds(-5));

        _orderRepoMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    public void CancelOrder_ValidOrder_UpdatesStatusToCancelled()
    {
        var order = BuildValidOrder();

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([order]);

        _orderRepoMock
            .Setup(r => r.Update(order));

        var result = _orderService.CancelOrder(order.OrderId);

        Assert.AreEqual(OrderStatus.Cancelled, order.OrderStatus);

        Assert.IsNotNull(result);
        Assert.AreEqual("Cancelled", result.Status);
        Assert.IsTrue(result.UpdatedAt <= DateTime.Now);
        Assert.IsTrue(result.UpdatedAt > DateTime.Now.AddSeconds(-5));

        _orderRepoMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CancelOrder_OrderNotFound_ThrowsException()
    {
        var orderId = 5;

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns([]);

            _orderService.CancelOrder(orderId);
    }
}

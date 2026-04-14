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
    private Mock<IOrderFactory> _orderFactoryMock = null!;
    private OrderService _orderService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _productRepoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        _promotionRepoMock = new Mock<IPromotionRepository>(MockBehavior.Strict);
        _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
        _shippingCalcMock = new Mock<IShippingCostCalculator>(MockBehavior.Strict);
        _orderFactoryMock = new Mock<IOrderFactory>(MockBehavior.Strict);

        _orderService = new OrderService(
            _orderRepoMock.Object,
            _productRepoMock.Object,
            _promotionRepoMock.Object,
            _userRepoMock.Object,
            _shippingCalcMock.Object,
            _orderFactoryMock.Object);
    }

    [TestMethod]
    public void OrderService_CreateOrder_Valid()
    {
        // Arrange
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
            code: "PROD-001",
            name: "Producto de prueba uno",
            description: "Descripcion valida del producto de prueba numero uno",
            line: "LineA",
            category: "CategoryA",
            images: "image1.jpg|10",
            active: true);
        product1.Price = 100m;

        var product2 = Product.Create(
            code: "PROD-002",
            name: "Producto de prueba dos",
            description: "Descripcion valida del producto de prueba numero dos",
            line: "LineB",
            category: "CategoryB",
            images: "image2.jpg|10",
            active: true);
        product2.Price = 200m;

        var items = new List<(string ProductCode, int Quantity)> { ("PROD-001", 2), ("PROD-002", 1) };

        // subtotal = (100 * 2) + (200 * 1) = 400
        // shipping = 50 (mockeado)
        // total = 400 + 50 = 450
        var expectedSubtotal = 400m;
        var expectedShipping = 50m;
        var expectedTotal = 450m;

        var createdOrder = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: Address.Create(street, doorNumber, apartment),
            products: new List<Product> { product1, product1, product2 },
            clientId: clientId,
            orderNumber: 1,
            subtotal: (double)expectedSubtotal,
            shippingCost: (double)expectedShipping,
            totalCost: (double)expectedTotal);

        _userRepoMock
            .Setup(r => r.GetById(clientId))
            .Returns(user);

        _productRepoMock
            .Setup(r => r.GetByCode("PROD-001"))
            .Returns(product1);

        _productRepoMock
            .Setup(r => r.GetByCode("PROD-002"))
            .Returns(product2);

        _shippingCalcMock
            .Setup(c => c.Calculate(DeliveryType.Express))
            .Returns(expectedShipping);

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

        _orderRepoMock
            .Setup(r => r.Add(It.IsAny<Order>()));

        // Act
        var result = _orderService.CreateOrder(
            clientId,
            DeliveryType.Express.ToString(),
            street,
            doorNumber,
            apartment,
            items);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual(expectedSubtotal, result.Subtotal);
        Assert.AreEqual(expectedShipping, result.ShippingCost);
        Assert.AreEqual(expectedTotal, result.Total);

        _userRepoMock.Verify(r => r.GetById(clientId), Times.Once);
        _productRepoMock.Verify(r => r.GetByCode("PROD-001"), Times.Once);
        _productRepoMock.Verify(r => r.GetByCode("PROD-002"), Times.Once);
        _shippingCalcMock.Verify(c => c.Calculate(DeliveryType.Express), Times.Once);
        _orderRepoMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }
}

using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Discounts;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.IDataAccess;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

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

        var promotion = Promotion.Create("10% off", 10, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today));
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

        var dto = new UpdateStatusEntryDTO("Prepared");

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

        var dto = new UpdateStatusEntryDTO("Prepared");

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

        var dto = new UpdateStatusEntryDTO("StatusInvalido");

        Assert.ThrowsException<ArgumentException>(() =>
            _orderService.UpdateStatus(order.OrderId, dto));
    }
}

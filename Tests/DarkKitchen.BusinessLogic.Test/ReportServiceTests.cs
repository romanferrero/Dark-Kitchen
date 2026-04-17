using System.Linq.Expressions;
using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ReportServiceTests
{
    private static readonly DateTime DateFrom = new(2026, 1, 1);
    private static readonly DateTime DateTo = new(2026, 12, 31);

    private Mock<IRepository<Order>> _orderRepoMock = null!;
    private Mock<IRepository<User>> _userRepoMock = null!;
    private ReportService _reportService = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepoMock = new Mock<IRepository<Order>>(MockBehavior.Strict);
        _userRepoMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _reportService = new ReportService(_orderRepoMock.Object, _userRepoMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_NoOrders_ReturnsEmptyList()
    {
        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(new List<Order>());

        var result = _reportService.GetTopProducts(DateFrom, DateTo);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopProducts_WithOrders_ReturnsProductsOrderedByQuantity()
    {
        var productA = CreateProduct("PROD01", "Producto AAA uno");
        var productB = CreateProduct("PROD02", "Producto BBB dos");

        var orders = new List<Order> { CreateOrder(1, [productA, productB]), CreateOrder(2, [productA]), };

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(orders);

        var result = _reportService.GetTopProducts(DateFrom, DateTo);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("PROD01", result[0].Code);
        Assert.AreEqual(2, result[0].QuantitySold);
        Assert.AreEqual("PROD02", result[1].Code);
        Assert.AreEqual(1, result[1].QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_MoreThanFiveProducts_ReturnsOnlyTopFive()
    {
        var products = Enumerable.Range(1, 7)
            .Select(i => CreateProduct($"PROD0{i}", $"Producto numero {i:D2}"))
            .ToList();

        var orders = Enumerable.Range(0, 7)
            .Select(i => CreateOrder(i, Enumerable.Repeat(products[i], i + 1).ToList()))
            .ToList();

        _orderRepoMock
            .Setup(r => r.GetAll(It.IsAny<Expression<Func<Order, bool>>>()))
            .Returns(orders);

        var result = _reportService.GetTopProducts(DateFrom, DateTo);

        Assert.AreEqual(5, result.Count);
    }

    [TestMethod]
    public void GetSalesReport_NoOrders_ReturnsEmptyReport()
    {
        _orderRepoMock
            .Setup(r => r.GetAll(null))
            .Returns([]);

        _userRepoMock
            .Setup(r => r.GetAll(null))
            .Returns([]);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(0, result.MonthlySales.Count);
        Assert.AreEqual(0, result.GrandTotal);
    }

    [TestMethod]
    public void GetSalesReport_WithOrders_ReturnsGroupedByMonthAndClient()
    {
        var product = CreateProduct("PROD01", "Producto AAA uno");

        var orders = new List<Order>
        {
            CreateOrder(1, [product], storeId: 1, clientId: 1, subtotal: 100, tax: 10, total: 110),
            CreateOrder(2, [product], storeId: 2, clientId: 2, subtotal: 200, tax: 10, total: 210),
        };

        var users = new List<User>
        {
            CreateClient("Juan", "Perez", "juan@test.com", "099111111"),
            CreateClient("Maria", "Lopez", "maria@test.com", "099222222"),
        };

        _orderRepoMock
            .Setup(r => r.GetAll(null))
            .Returns(orders);

        _userRepoMock
            .Setup(r => r.GetAll(null))
            .Returns(users);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(1, result.MonthlySales.Count);
        Assert.AreEqual(2, result.MonthlySales[0].ClientSales.Count);
        Assert.AreEqual(320, result.GrandTotal);
    }

    private static Product CreateProduct(string code, string name)
    {
        return Product.Create(
            code,
            name,
            $"Descripcion larga de {name}",
            "Linea1",
            "Cat1",
            $"http://img.com/{code}.jpg",
            true);
    }

    private static User CreateClient(string name, string lastName, string email, string phone)
    {
        return User.CreateClient(
            name,
            lastName,
            email,
            phone,
            "ValidPass@1Ab!xyz");
    }

    private static Address CreateAddress()
    {
        return Address.Create("Calle", "123", "Apt");
    }

    private static Order CreateOrder(
        int id,
        List<Product> products,
        int storeId = 1,
        int clientId = 1,
        double subtotal = 100,
        double tax = 10,
        double total = 110)
    {
        return Order.Create(
            id,
            DeliveryType.Express,
            CreateAddress(),
            products,
            storeId,
            clientId,
            subtotal,
            tax,
            total);
    }
}

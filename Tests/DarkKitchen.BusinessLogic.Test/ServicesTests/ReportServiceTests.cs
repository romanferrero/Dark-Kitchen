using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class ReportServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private ReportService _reportService = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        _userRepositoryMock = new Mock<IRepository<User>>(MockBehavior.Strict);
        _reportService = new ReportService(_orderRepositoryMock.Object, _userRepositoryMock.Object);
    }

    private static Product CreateProduct(
        string code,
        string name,
        string imageUrl)
    {
        return Product.Create(
            code,
            name,
            100m,
            "Descripcion del producto test",
            "Minutas clásicas",
            "Fritos",
            imageUrl,
            true);
    }

    private static OrderProduct ToOrderProduct(Product product, int quantity = 1)
    {
        return new OrderProduct
        {
            ProductId = product.Id,
            Product = product,
            Quantity = quantity
        };
    }

    private static Order CreateOrder(
        int clientId,
        List<OrderProduct> orderProducts,
        DateTime date,
        decimal totalCost = 150.0m)
    {
        var order = Order.Create(
            DeliveryType.Express,
            Address.Create("Calle", "123", "Apto 1"),
            orderProducts, clientId, 0, 100.0m, 50.0m, totalCost);
        order.OrderDate = date;
        return order;
    }

    [TestMethod]
    public void GetTopProducts_NoOrdersInRange_ReturnsEmptyList()
    {
        _orderRepositoryMock
            .Setup(r => r.GetOrdersWithProducts(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns([]);

        var result = _reportService.GetTopProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopProducts_WithOrders_ReturnsProductsOrderedByQuantity()
    {
        var productA = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");
        var productB = CreateProduct("PRODB", "Pizza Muzzarella Grande", "http://img.com/pizza.jpg");

        var orders = new List<Order>
        {
            CreateOrder(1, [ToOrderProduct(productA), ToOrderProduct(productB)], new DateTime(2026, 1, 10)),
            CreateOrder(2, [ToOrderProduct(productA)], new DateTime(2026, 1, 15))
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrdersWithProducts(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(orders);

        var result = _reportService.GetTopProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("PRODA", result[0].Code);
        Assert.AreEqual(2, result[0].QuantitySold);
        Assert.AreEqual("PRODB", result[1].Code);
        Assert.AreEqual(1, result[1].QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_WithImages_ReturnsDistinctImageUrls()
    {
        var product = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");
        var orders = new List<Order> { CreateOrder(1, [ToOrderProduct(product)], new DateTime(2026, 1, 10)) };

        _orderRepositoryMock
            .Setup(r => r.GetOrdersWithProducts(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(orders);

        var result = _reportService.GetTopProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].ImageUrls.Contains("http://img.com/burger.jpg"));
    }

    [TestMethod]
    public void GetTopProducts_MoreThanFiveProducts_ReturnsOnlyTopFive()
    {
        var orders = new List<Order>();
        for(var i = 1; i <= 7; i++)
        {
            var product = CreateProduct($"PROD{i:D2}", $"Producto numero {i:D2}", $"http://img.com/p{i}.jpg");
            orders.Add(CreateOrder(1, [ToOrderProduct(product, i)], new DateTime(2026, 1, 10)));
        }

        _orderRepositoryMock
            .Setup(r => r.GetOrdersWithProducts(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(orders);

        var result = _reportService.GetTopProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(5, result.Count);
        Assert.AreEqual("PROD07", result[0].Code);
        Assert.AreEqual(7, result[0].QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_CallsRepositoryWithCorrectDates()
    {
        var dateFrom = new DateTime(2026, 2, 1);
        var dateTo = new DateTime(2026, 2, 28);

        _orderRepositoryMock
            .Setup(r => r.GetOrdersWithProducts(dateFrom, dateTo))
            .Returns([]);

        _reportService.GetTopProducts(dateFrom, dateTo);

        _orderRepositoryMock.Verify(
            r => r.GetOrdersWithProducts(dateFrom, dateTo),
            Times.Once);
    }

    [TestMethod]
    public void GetTopProducts_WithQuantity_SumsCorrectly()
    {
        var productA = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var orders = new List<Order>
        {
            CreateOrder(1, [ToOrderProduct(productA, 5)], new DateTime(2026, 1, 10)),
            CreateOrder(2, [ToOrderProduct(productA, 3)], new DateTime(2026, 1, 15))
        };

        _orderRepositoryMock
            .Setup(r => r.GetOrdersWithProducts(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(orders);

        var result = _reportService.GetTopProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(8, result[0].QuantitySold);
    }

    [TestMethod]
    public void GetSalesReport_NoOrders_ReturnsEmptyReport()
    {
        _userRepositoryMock.Setup(r => r.GetAll(null)).Returns([]);
        _orderRepositoryMock.Setup(r => r.GetAll(null)).Returns([]);

        var result = _reportService.GetSalesReport();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.MonthlySales.Count);
        Assert.AreEqual(0m, result.GrandTotal);
    }

    [TestMethod]
    public void GetSalesReport_WithOrders_CalculatesGrandTotalCorrectly()
    {
        var user1 = User.CreateClient("Juan", "Perez", "juan@test.com", "099123456", "Passw0rd!abcdefg");
        user1.Id = 1;
        var user2 = User.CreateClient("Yuri", "Gagarin", "yuri@test.com", "099654321", "Passw0rd!abcdefg");
        user2.Id = 2;
        var user3 = User.CreateClient("Sommer", "Schutman", "sommer@test.com", "099111111", "Passw0rd!abcdefg");
        user3.Id = 3;

        _userRepositoryMock.Setup(r => r.GetAll(null)).Returns([user1, user2, user3]);

        var product = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var orders = new List<Order>
        {
            CreateOrder(1, [ToOrderProduct(product)], new DateTime(2026, 1, 10), 5000m),
            CreateOrder(2, [ToOrderProduct(product)], new DateTime(2026, 1, 20), 4000m),
            CreateOrder(1, [ToOrderProduct(product)], new DateTime(2026, 2, 10), 1000m),
            CreateOrder(3, [ToOrderProduct(product)], new DateTime(2026, 2, 15), 5600m)
        };

        _orderRepositoryMock.Setup(r => r.GetAll(null)).Returns(orders);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(15600m, result.GrandTotal);
    }

    [TestMethod]
    public void GetSalesReport_WithOrders_ReturnsCorrectMonthlyStructure()
    {
        var user1 = User.CreateClient("Juan", "Perez", "juan@test.com", "099123456", "Passw0rd!abcdefg");
        user1.Id = 1;
        var user2 = User.CreateClient("Yuri", "Gagarin", "yuri@test.com", "099654321", "Passw0rd!abcdefg");
        user2.Id = 2;

        _userRepositoryMock.Setup(r => r.GetAll(null)).Returns([user1, user2]);

        var product = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var orders = new List<Order>
        {
            CreateOrder(1, [ToOrderProduct(product)], new DateTime(2026, 1, 10), 5000m),
            CreateOrder(2, [ToOrderProduct(product)], new DateTime(2026, 1, 20), 4000m)
        };

        _orderRepositoryMock.Setup(r => r.GetAll(null)).Returns(orders);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(1, result.MonthlySales.Count);
        var firstMonth = result.MonthlySales[0];
        Assert.AreEqual("2026-01", firstMonth.Period);
        Assert.AreEqual(9000m, firstMonth.MonthlyTotal);
        Assert.AreEqual(2, firstMonth.ClientSales.Count);
        Assert.AreEqual("Juan Perez", firstMonth.ClientSales[0].ClientName);
        Assert.AreEqual(5000m, firstMonth.ClientSales[0].Total);
    }

    [TestMethod]
    public void GetSalesReport_UnknownClient_ShowsClienteWithId()
    {
        _userRepositoryMock.Setup(r => r.GetAll(null)).Returns([]);

        var product = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");
        var orders = new List<Order> { CreateOrder(999, [ToOrderProduct(product)], new DateTime(2026, 1, 10), 3000m) };

        _orderRepositoryMock.Setup(r => r.GetAll(null)).Returns(orders);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(1, result.MonthlySales.Count);
        Assert.AreEqual("Cliente 999", result.MonthlySales[0].ClientSales[0].ClientName);
        Assert.AreEqual(3000m, result.MonthlySales[0].ClientSales[0].Total);
    }

    [TestMethod]
    public void GetSalesReport_MultipleMonths_ReturnsMultiplePeriods()
    {
        var user1 = User.CreateClient("Juan", "Perez", "juan@test.com", "099123456", "Passw0rd!abcdefg");
        user1.Id = 1;

        _userRepositoryMock.Setup(r => r.GetAll(null)).Returns([user1]);

        var product = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var orders = new List<Order>
        {
            CreateOrder(1, [ToOrderProduct(product)], new DateTime(2026, 1, 10), 5000m),
            CreateOrder(1, [ToOrderProduct(product)], new DateTime(2026, 2, 10), 1000m)
        };

        _orderRepositoryMock.Setup(r => r.GetAll(null)).Returns(orders);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(2, result.MonthlySales.Count);
        Assert.AreEqual("2026-01", result.MonthlySales[0].Period);
        Assert.AreEqual(5000m, result.MonthlySales[0].MonthlyTotal);
        Assert.AreEqual("2026-02", result.MonthlySales[1].Period);
        Assert.AreEqual(1000m, result.MonthlySales[1].MonthlyTotal);
    }
}

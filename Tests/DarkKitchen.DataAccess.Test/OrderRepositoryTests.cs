using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test;

[TestClass]
public class OrderRepositoryTests
{
    private AppDbContext _context = null!;
    private OrderRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        _repository = new OrderRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private Product CreateProduct(string code, string name, string imageUrl)
    {
        return Product.Create(
            code, name, "Descripcion del producto test",
            "Minutas clásicas", "Fritos", $"{imageUrl}|100", true);
    }

    private Order CreateOrder(int id, int clientId, List<Product> products, DateTime date)
    {
        var order = Order.Create(
            id, DeliveryType.Express,
            Address.Create("Calle", id.ToString(), "Apto 1"),
            products, clientId, id, 100.0, 50.0, 150.0);
        order.OrderDate = date;
        return order;
    }

    [TestMethod]
    public void GetTopSellingProducts_NoOrdersInRange_ReturnsEmptyList()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);

        var result = _repository.GetTopSellingProducts(dateFrom, dateTo, 5);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopSellingProducts_WithOrders_ReturnsProductsOrderedByQuantity()
    {
        var productA1 = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");
        var productB1 = CreateProduct("PRODB", "Pizza Muzzarella Grande", "http://img.com/pizza.jpg");
        var productA2 = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var order1 = CreateOrder(1, 1, new List<Product> { productA1, productB1 }, new DateTime(2026, 1, 10));
        var order2 = CreateOrder(2, 2, new List<Product> { productA2 }, new DateTime(2026, 1, 15));

        _context.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var result = _repository.GetTopSellingProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31), 5);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("PRODA", result[0].Code);
        Assert.AreEqual(2, result[0].QuantitySold);
        Assert.AreEqual("PRODB", result[1].Code);
        Assert.AreEqual(1, result[1].QuantitySold);
    }

    [TestMethod]
    public void GetMonthlySalesGroupedByClient_NoOrders_ReturnsEmptyList()
    {
        var users = new List<User>();

        var result = _repository.GetMonthlySalesGroupedByClient(users);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetMonthlySalesGroupedByClient_WithOrders_GroupsByMonthAndClient()
    {
        var user1 = User.CreateClient("Juan", "Perez", "juan@test.com", "099123456", "Passw0rd!abcdefg");
        var user2 = User.CreateClient("Yuri", "Gagarin", "yuri@test.com", "099654321", "Passw0rd!abcdefg");

        _context.Users.AddRange(user1, user2);
        _context.SaveChanges();

        var productA = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var order1 = CreateOrder(1, user1.Id, new List<Product> { productA }, new DateTime(2026, 1, 10));
        order1.TotalCost = 5000.0;

        var productB = CreateProduct("PRODB", "Pizza Muzzarella Grande", "http://img.com/pizza.jpg");

        var order2 = CreateOrder(2, user2.Id, new List<Product> { productB }, new DateTime(2026, 1, 20));
        order2.TotalCost = 4000.0;

        _context.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var users = _context.Users.ToList();

        var result = _repository.GetMonthlySalesGroupedByClient(users);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("2026-01", result[0].Period);
        Assert.AreEqual(9000m, result[0].MonthlyTotal);
        Assert.AreEqual(2, result[0].ClientSales.Count);
        Assert.AreEqual("Juan Perez", result[0].ClientSales[0].ClientName);
        Assert.AreEqual(5000m, result[0].ClientSales[0].Total);
        Assert.AreEqual("Yuri Gagarin", result[0].ClientSales[1].ClientName);
        Assert.AreEqual(4000m, result[0].ClientSales[1].Total);
    }

    [TestMethod]
    public void GetMonthlySalesGroupedByClient_WithMultipleMonths_ReturnsMultiplePeriods()
    {
        var user1 = User.CreateClient("Juan", "Perez", "juan@test.com", "099123456", "Passw0rd!abcdefg");

        _context.Users.Add(user1);
        _context.SaveChanges();

        var productA = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var order1 = CreateOrder(1, user1.Id, new List<Product> { productA }, new DateTime(2026, 1, 10));
        order1.TotalCost = 5000.0;

        var productB = CreateProduct("PRODB", "Pizza Muzzarella Grande", "http://img.com/pizza.jpg");

        var order2 = CreateOrder(2, user1.Id, new List<Product> { productB }, new DateTime(2026, 2, 15));
        order2.TotalCost = 1000.0;

        _context.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var users = _context.Users.ToList();

        var result = _repository.GetMonthlySalesGroupedByClient(users);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("2026-01", result[0].Period);
        Assert.AreEqual(5000m, result[0].MonthlyTotal);
        Assert.AreEqual("2026-02", result[1].Period);
        Assert.AreEqual(1000m, result[1].MonthlyTotal);
    }
}

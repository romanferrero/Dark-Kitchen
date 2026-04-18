using DarkKitchen.DataAccess;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
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
        var productA = Product.Create(
            "PRODA", "Hamburguesa Clásica", "Descripcion del producto test",
            "Minutas clásicas", "Fritos", "http://img.com/burger.jpg|100", true);
        var productB = Product.Create(
            "PRODB", "Pizza Muzzarella Grande", "Descripcion del producto test",
            "Minutas clásicas", "Fritos", "http://img.com/pizza.jpg|100", true);

        _context.Products.AddRange(productA, productB);
        _context.SaveChanges();

        var order1 = Order.Create(1, DeliveryType.Express,
            Address.Create("Calle", "123", "Apto 1"),
            new List<Product> { productA, productB }, 1, 1, 100.0, 50.0, 150.0);
        order1.OrderDate = new DateTime(2026, 1, 10);

        var order2 = Order.Create(2, DeliveryType.Express,
            Address.Create("Calle", "456", "Apto 2"),
            new List<Product> { productA }, 2, 2, 100.0, 50.0, 150.0);
        order2.OrderDate = new DateTime(2026, 1, 15);

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
}

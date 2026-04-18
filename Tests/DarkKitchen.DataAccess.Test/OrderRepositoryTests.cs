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
}

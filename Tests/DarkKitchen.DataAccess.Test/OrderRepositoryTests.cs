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
    public void Initialize()
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
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsInDatabase()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg|100",
            active: true);
        product.Price = 200m;

        _context.Products.Add(product);
        _context.SaveChanges();

        var address = new Address
        {
            Street = "18 de Julio",
            DoorNumber = "1234",
            Apartment = "Apto 101",
        };

        var items = new List<OrderItem>
    {
        new OrderItem
        {
            ProductId = product.Id,
            Product = product,
            Quantity = 2,
            UnitPrice = 200m,
        },
    };

        var order = Order.Create(
            clientId: 1,
            deliveryType: DeliveryType.Express,
            address: address,
            items: items);

        _repository.Add(order);

        var saved = _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(OrderStatus.Pending, saved.Status);
        Assert.AreEqual(1, saved.ClientId);
        Assert.AreEqual(1, saved.Items.Count);
    }
}

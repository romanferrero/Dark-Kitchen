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

    private Product SeedProduct()
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

        return product;
    }

    private User SeedUser()
    {
        var user = new User
        {
            FirstName = "Roman",
            LastName = "Ferrero",
            Email = "roman@test.com",
            Phone = "099123456",
            Password = "Password15365!!",
            Role = UserRole.Client
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    private Order CreateValidOrder(Product product, int clientId)
    {
        var address = Address.Create("18 de Julio", "1234", "Apto 101");

        var items = new List<Product> { product };

        return Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: items,
            clientId: clientId,
            orderNumber: 1,
            subtotal: 200.0,
            shippingCost: 50.0,
            totalCost: 250.0);
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsInDatabase()
    {
        var user = SeedUser();
        var product = SeedProduct();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        var saved = _context.Orders
            .Include(o => o.Products)
            .FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(OrderStatus.Pending, saved.OrderStatus);
        Assert.AreEqual(user.Id, saved.ClientId);
        Assert.AreEqual(1, saved.Products.Count);
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsAddress()
    {
        var product = SeedProduct();
        var user = SeedUser();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        var saved = _context.Orders.FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual("18 de Julio", saved.Address.Street);
        Assert.AreEqual("1234", saved.Address.DoorNumber);
        Assert.AreEqual("Apto 101", saved.Address.Apartment);
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsProducts()
    {
        var product = SeedProduct();
        var user = SeedUser();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        var saved = _context.Orders
            .Include(o => o.Products)
            .FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(1, saved.Products.Count);
        Assert.AreEqual(product.Id, saved.Products[0].Id);
        Assert.AreEqual(200m, saved.Products[0].Price);
    }

    [TestMethod]
    public void Add_ValidOrder_GeneratesId()
    {
        var product = SeedProduct();
        var user = SeedUser();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        Assert.IsTrue(order.Id > 0);
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsDeliveryType()
    {
        var product = SeedProduct();
        var user = SeedUser();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        var saved = _context.Orders.FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(DeliveryType.Express, saved.DeliveryType);
    }
}

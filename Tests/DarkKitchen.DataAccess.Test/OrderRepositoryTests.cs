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

        return Order.Create(
            clientId: clientId,
            deliveryType: DeliveryType.Express,
            address: address,
            items: items);
    }

    [TestMethod]
    public void Add_ValidOrder_PersistsInDatabase()
    {
        var user = SeedUser();
        var product = SeedProduct();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        var saved = _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(OrderStatus.Pending, saved.Status);
        Assert.AreEqual(user.Id, saved.ClientId);
        Assert.AreEqual(1, saved.Items.Count);
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
    public void Add_ValidOrder_PersistsOrderItems()
    {
        var product = SeedProduct();
        var user = SeedUser();
        var order = CreateValidOrder(product, user.Id);

        _repository.Add(order);

        var saved = _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual(1, saved.Items.Count);
        Assert.AreEqual(2, saved.Items[0].Quantity);
        Assert.AreEqual(200m, saved.Items[0].UnitPrice);
        Assert.AreEqual(product.Id, saved.Items[0].ProductId);
    }
}

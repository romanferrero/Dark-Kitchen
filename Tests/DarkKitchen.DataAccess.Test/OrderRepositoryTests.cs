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

        Assert.IsTrue(order.OrderId > 0);
    }

    [TestMethod]
    public void GetClientOrders_FiltersOrdersByClientId()
    {
        var user1 = SeedUser();
        var user2 = new User
        {
            FirstName = "Maria",
            LastName = "Lopez",
            Email = "maria@test.com",
            Phone = "099000000",
            Password = "Password15365!!",
            Role = UserRole.Client
        };
        _context.Users.Add(user2);
        _context.SaveChanges();

        var product = SeedProduct();
        var order1 = CreateValidOrder(product, user1.Id);
        var order2 = CreateValidOrder(product, user2.Id);

        _repository.Add(order1);
        _repository.Add(order2);

        var result = _repository.GetClientOrders(user1.Id, null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(user1.Id, result[0].ClientId);
    }

    [TestMethod]
    public void GetOrdersByDateRange_RangeIncludesToday_ReturnsOrder()
    {
        var user = SeedUser();
        var product = SeedProduct();
        var order = CreateValidOrder(product, user.Id);
        _repository.Add(order);

        var from = DateTime.Today.AddDays(-1);
        var to = DateTime.Today.AddDays(1);

        var result = _repository.GetOrdersByDateRange(from, to, null, null);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetOrdersByDateRange_RangeExcludesToday_ReturnsEmpty()
    {
        var user = SeedUser();
        var product = SeedProduct();
        var order = CreateValidOrder(product, user.Id);
        _repository.Add(order);

        var from = DateTime.Today.AddDays(-10);
        var to = DateTime.Today.AddDays(-5);

        var result = _repository.GetOrdersByDateRange(from, to, null, null);

        Assert.AreEqual(0, result.Count);
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

    [TestMethod]
    public void GetOrdersByDateRange_WithWhitespaceStreet_DoesNotApplyStreetFilter()
    {
        var user = SeedUser();
        var product = SeedProduct();

        var order1 = CreateValidOrder(product, user.Id);
        var order2 = CreateValidOrder(product, user.Id);
        order2.OrderNumber = 2;
        order2.Address = Address.Create("Bv. Artigas", "500", null);

        _repository.Add(order1);
        _repository.Add(order2);

        var from = DateTime.Today.AddDays(-1);
        var to = DateTime.Today.AddDays(1);

        var result = _repository.GetOrdersByDateRange(from, to, "   ", null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetClientOrders_WithFilters_ReturnsNewestFirst()
    {
        var user = SeedUser();
        var product = SeedProduct();

        var older = CreateValidOrder(product, user.Id);
        older.OrderNumber = 20;
        older.OrderStatus = OrderStatus.Prepared;
        older.OrderDate = DateTime.Today.AddDays(-1);

        var newer = CreateValidOrder(product, user.Id);
        newer.OrderNumber = 21;
        newer.OrderStatus = OrderStatus.Prepared;
        newer.OrderDate = DateTime.Today;

        _repository.Add(older);
        _repository.Add(newer);

        var result = _repository.GetClientOrders(
            user.Id,
            DateTime.Today.AddDays(-2),
            DateTime.Today.AddDays(1),
            OrderStatus.Prepared);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(21, result[0].OrderNumber);
        Assert.AreEqual(20, result[1].OrderNumber);
    }

    [TestMethod]
    public void GetOrdersByDateRange_WithStreetAndStatus_ReturnsNewestFirst()
    {
        var user = SeedUser();
        var product = SeedProduct();

        var older = CreateValidOrder(product, user.Id);
        older.OrderNumber = 30;
        older.OrderStatus = OrderStatus.Prepared;
        older.OrderDate = DateTime.Today.AddDays(-1);

        var newer = CreateValidOrder(product, user.Id);
        newer.OrderNumber = 31;
        newer.OrderStatus = OrderStatus.Prepared;
        newer.OrderDate = DateTime.Today;

        var differentStatus = CreateValidOrder(product, user.Id);
        differentStatus.OrderNumber = 32;
        differentStatus.OrderStatus = OrderStatus.Pending;
        differentStatus.OrderDate = DateTime.Today;

        _repository.Add(older);
        _repository.Add(newer);
        _repository.Add(differentStatus);

        var result = _repository.GetOrdersByDateRange(
            DateTime.Today.AddDays(-2),
            DateTime.Today.AddDays(1),
            "Julio",
            OrderStatus.Prepared);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(31, result[0].OrderNumber);
        Assert.AreEqual(30, result[1].OrderNumber);
    }

    [TestMethod]
    public void GetOrderById_ReturnsProductsSortedByCode()
    {
        var user = SeedUser();

        var productB = Product.Create(
            code: "PIZZA1",
            name: "Pizza clasica",
            description: "Pizza de muzzarella tradicional",
            line: "Pizzas",
            category: "Horno",
            images: "http://img.com/pizza1.jpg|100",
            active: true);

        var productA = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            description: "Hamburguesa con lechuga y tomate fresco",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg|100",
            active: true);

        _context.Products.Add(productB);
        _context.Products.Add(productA);
        _context.SaveChanges();

        var address = Address.Create("18 de Julio", "1234", "Apto 101");
        var order = Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: address,
            products: [productB, productA],
            clientId: user.Id,
            orderNumber: 90,
            subtotal: 400.0,
            shippingCost: 50.0,
            totalCost: 550.0);

        _repository.Add(order);

        var result = _repository.GetOrderById(order.OrderId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Products.Count);
        Assert.AreEqual("BURG01", result.Products[0].Code);
        Assert.AreEqual("PIZZA1", result.Products[1].Code);
    }
}

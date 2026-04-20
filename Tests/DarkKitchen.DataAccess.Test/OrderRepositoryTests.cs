using DarkKitchen.DataAccess.Context;
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

    private Product CreateProduct(string code, string name, string imageUrl)
    {
        return Product.Create(
            code, name, "Descripcion del producto test",
            "Minutas clásicas", "Fritos", $"{imageUrl}|100", true);
    }

    private Order CreateOrder(int id, int clientId, List<Product> products, DateTime date, decimal totalCost = 150.0m)
    {
        var order = Order.Create(
            id, DeliveryType.Express,
            Address.Create("Calle", id.ToString(), "Apto 1"),
            products, clientId, id, 100.0m, 50.0m, totalCost);
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

        var order1 = CreateOrder(1, 1, [productA1, productB1], new DateTime(2026, 1, 10));
        var order2 = CreateOrder(2, 2, [productA2], new DateTime(2026, 1, 15));

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

        var order1 = CreateOrder(1, user1.Id, [productA], new DateTime(2026, 1, 10), 5000.0m);

        var productB = CreateProduct("PRODB", "Pizza Muzzarella Grande", "http://img.com/pizza.jpg");

        var order2 = CreateOrder(2, user2.Id, [productB], new DateTime(2026, 1, 20), 4000.0m);

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

        var order1 = CreateOrder(1, user1.Id, [productA], new DateTime(2026, 1, 10), 5000.0m);

        var productB = CreateProduct("PRODB", "Pizza Muzzarella Grande", "http://img.com/pizza.jpg");

        var order2 = CreateOrder(2, user1.Id, [productB], new DateTime(2026, 2, 10), 1000.0m);

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
        var productA = CreateProduct("PRODA", "Hamburguesa Clásica", "http://img.com/burger.jpg");

        var order1 = CreateOrder(1, 999, [productA], new DateTime(2026, 1, 10), 3000.0m);

        _context.Orders.Add(order1);
        _context.SaveChanges();

        var users = new List<User>();

        var result = _repository.GetMonthlySalesGroupedByClient(users);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Cliente 999", result[0].ClientSales[0].ClientName);
        Assert.AreEqual(3000m, result[0].ClientSales[0].Total);
    }

    [TestMethod]
    public void GetOrdersByDateRange_WithWhitespaceStreet_DoesNotApplyStreetFilter()
    {
        var user = SeedUser();
        var product = SeedProduct();

        var order1 = CreateValidOrder(product, user.Id);
        var order2 = CreateValidOrder(product, user.Id);
        order2.OrderNumber = 2;
        order2.Address = Address.Create("Bv. Artigas", "500", string.Empty);

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
            subtotal: 400.0m,
            shippingCost: 50.0m,
            totalCost: 550.0m);

        _repository.Add(order);

        var result = _repository.GetOrderById(order.OrderId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Products.Count);
        Assert.AreEqual("BURG01", result.Products[0].Code);
        Assert.AreEqual("PIZZA1", result.Products[1].Code);
    }

    private User SeedUser()
    {
        var user = User.CreateClient("Carlos", "Suarez", "carlos@test.com", "099111222", "Passw0rd!abcdefg");
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    private Product SeedProduct()
    {
        var product = CreateProduct("PRODX", "Hamburguesa Doble", "http://img.com/prodx.jpg");
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    private Order CreateValidOrder(Product product, int clientId)
    {
        var orderNumber = _context.Orders.Count() + 1;
        return Order.Create(
            orderId: 0,
            deliveryType: DeliveryType.Express,
            address: Address.Create("18 de Julio", "1234", "Apto 1"),
            products: [product],
            clientId: clientId,
            orderNumber: orderNumber,
            subtotal: 400.0m,
            shippingCost: 50.0m,
            totalCost: 450.0m);
    }
}

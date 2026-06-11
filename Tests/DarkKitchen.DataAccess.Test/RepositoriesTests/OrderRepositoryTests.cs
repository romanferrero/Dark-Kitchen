using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.RepositoriesTests;

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
        return Product.Create(new CreateProductParamsDto(
            code,
            name,
            100m,
            "Test product description",
            "Classic snacks",
            "Fried",
            imageUrl,
            true));
    }

    private static List<OrderProduct> ToOrderProducts(Product product, int quantity = 1)
    {
        return [new OrderProduct { ProductId = product.Id, Product = product, Quantity = quantity }];
    }

    private static List<OrderProduct> ToOrderProducts(params Product[] products)
    {
        return products.Select(p => new OrderProduct
        {
            ProductId = p.Id,
            Product = p,
            Quantity = 1
        }).ToList();
    }

    private Order CreateOrder(int clientId, List<OrderProduct> orderProducts, DateTime date,
        int orderNumber = 0, decimal totalCost = 150.0m)
    {
        var order = Order.Create(new CreateOrderParamsDto(
            "Express",
            Address.Create("Main Street", "1234", "Apto 1"),
            orderProducts, clientId, orderNumber, 100.0m, 50.0m, totalCost));
        order.OrderDate = date;
        return order;
    }

    [TestMethod]
    public void GetOrdersWithProducts_NoOrdersInRange_ReturnsEmptyList()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);

        var result = _repository.GetOrdersWithProducts(dateFrom, dateTo);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetOrdersWithProducts_WithOrdersInRange_ReturnsOrders()
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

        var productA = CreateProduct("PRODA", "Classic Burger", "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==");
        var productB = CreateProduct("PRODB", "Pizza Muzzarella Grande", "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==");

        var order1 = CreateOrder(user1.Id, ToOrderProducts(productA), new DateTime(2026, 1, 10), 1);
        var order2 = CreateOrder(user2.Id, ToOrderProducts(productB), new DateTime(2026, 1, 15), 2);

        _context.Orders.AddRange(order1, order2);
        _context.SaveChanges();

        var result = _repository.GetOrdersWithProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetOrdersWithProducts_IncludesProductsAndImages()
    {
        var product = CreateProduct("PRODA", "Classic Burger", "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==");
        var order = CreateOrder(1, ToOrderProducts(product), new DateTime(2026, 1, 10), 1);

        _context.Orders.Add(order);
        _context.SaveChanges();

        var result = _repository.GetOrdersWithProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(1, result[0].Products.Count);
        Assert.AreEqual("PRODA", result[0].Products[0].Product.Code);
        Assert.IsTrue(result[0].Products[0].Product.Images.Count > 0);
    }

    [TestMethod]
    public void GetOrdersWithProducts_OutOfRange_ReturnsEmpty()
    {
        var product = CreateProduct("PRODA", "Classic Burger", "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==");
        var order = CreateOrder(1, ToOrderProducts(product), new DateTime(2026, 3, 10), 1);

        _context.Orders.Add(order);
        _context.SaveChanges();

        var result = _repository.GetOrdersWithProducts(
            new DateTime(2026, 1, 1), new DateTime(2026, 1, 31));

        Assert.AreEqual(0, result.Count);
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
    public void GetOrdersByDateRange_WithWhitespaceStreet_DoesNotApplyStreetFilter()
    {
        var user = SeedUser();
        var product = SeedProduct();

        var order1 = CreateValidOrder(product, user.Id);
        var order2 = CreateValidOrder(product, user.Id);
        order2.OrderNumber = 2;
        order2.Address = Address.Create("Elm Avenue", "500", string.Empty);

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
        older.UpdateStatus("Prepared");
        older.OrderDate = DateTime.Today.AddDays(-1);

        var newer = CreateValidOrder(product, user.Id);
        newer.OrderNumber = 21;
        newer.UpdateStatus("Prepared");
        newer.OrderDate = DateTime.Today;

        _repository.Add(older);
        _repository.Add(newer);

        var result = _repository.GetClientOrders(
            user.Id,
            DateTime.Today.AddDays(-2),
            DateTime.Today.AddDays(1),
            "Prepared");

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
        older.UpdateStatus("Prepared");
        older.OrderDate = DateTime.Today.AddDays(-1);

        var newer = CreateValidOrder(product, user.Id);
        newer.OrderNumber = 31;
        newer.UpdateStatus("Prepared");
        newer.OrderDate = DateTime.Today;

        var differentStatus = CreateValidOrder(product, user.Id);
        differentStatus.OrderNumber = 32;
        differentStatus.OrderDate = DateTime.Today;

        _repository.Add(older);
        _repository.Add(newer);
        _repository.Add(differentStatus);

        var result = _repository.GetOrdersByDateRange(
            DateTime.Today.AddDays(-2),
            DateTime.Today.AddDays(1),
            "Main",
            "Prepared");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(31, result[0].OrderNumber);
        Assert.AreEqual(30, result[1].OrderNumber);
    }

    [TestMethod]
    public void GetOrderById_ReturnsProductsSortedByCode()
    {
        var user = SeedUser();

        var productB = Product.Create(new CreateProductParamsDto(
            Code: "PIZZA1",
            Name: "Classic pizza",
            Price: 200m,
            Description: "Traditional mozzarella pizza",
            Line: "Pizzas",
            Category: "Horno",
            Images: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==",
            Active: true));

        var productA = Product.Create(new CreateProductParamsDto(
            Code: "BURG01",
            Name: "Classic burger",
            Price: 150m,
            Description: "Burger with lettuce and fresh tomato",
            Line: "Combo burgers",
            Category: "Parrilla",
            Images: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==",
            Active: true));

        _context.Products.Add(productB);
        _context.Products.Add(productA);
        _context.SaveChanges();

        var address = Address.Create("Main Street", "1234", "Apto 101");
        var order = Order.Create(new CreateOrderParamsDto(
            DeliveryName: "Express",
            Address: address,
            Products: ToOrderProducts(productB, productA),
            ClientId: user.Id,
            OrderNumber: 90,
            Subtotal: 400.0m,
            ShippingCost: 50.0m,
            TotalCost: 550.0m));

        _repository.Add(order);

        var result = _repository.GetOrderById(order.OrderId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Products.Count);
        Assert.AreEqual("BURG01", result.Products[0].Product.Code);
        Assert.AreEqual("PIZZA1", result.Products[1].Product.Code);
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
        var product = CreateProduct("PRODX", "Double Burger", "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==");
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    private Order CreateValidOrder(Product product, int clientId)
    {
        var orderNumber = _context.Orders.Count() + 1;
        return Order.Create(new CreateOrderParamsDto(
            DeliveryName: "Express",
            Address: Address.Create("Main Street", "1234", "Apto 1"),
            Products: ToOrderProducts(product),
            ClientId: clientId,
            OrderNumber: orderNumber,
            Subtotal: 400.0m,
            ShippingCost: 50.0m,
            TotalCost: 450.0m));
    }
}

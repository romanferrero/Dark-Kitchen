using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.RepositoriesTests;

[TestClass]
public class RepositoryTests
{
    private AppDbContext _context = null!;
    private Repository<Product> _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repo = new Repository<Product>(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public void Delete_WithMatchingPredicate_RemovesProduct()
    {
        var product = Product.Create(
            "PROD01",
            "Hamburguesa Clásica",
            250,
            "Hamburguesa con queso y lechuga fresca",
            "Combo burgers",
            "Parrilla",
            "hamburguesa.jpg|100",
            true);

        _context.Products.Add(product);
        _context.SaveChanges();

        _repo.Delete(p => p.Code == "PROD01");

        var remaining = _context.Products.ToList();

        Assert.AreEqual(0, remaining.Count);
    }

    [TestMethod]
    public void Delete_WithNotMatch_DoesNoRemoveAnything()
    {
        var product = Product.Create(
            "PROD02",
            "Hamburguesa Clásica",
            250,
            "Hamburguesa con queso y lechuga fresca",
            "Combo burgers",
            "Parrilla",
            "hamburguesa.jpg|200",
            true);

        _context.Products.Add(product);
        _context.SaveChanges();

        _repo.Delete(p => p.Code == "NOEXISTE");

        var remaining = _context.Products.ToList();

        Assert.AreEqual(1, remaining.Count);
        Assert.AreEqual("PROD02", remaining[0].Code);
    }

    [TestMethod]
    public void Delete_WithMultipleMatches_RemovesAllMatching()
    {
        var product1 = Product.Create(
            "PROD03",
            "Milanesa Napolitana Especial",
            400,
            "Milanesa con jamón queso y salsa",
            "Minutas clásicas",
            "Fritos",
            "mila1.jpg|150",
            true);

        var product2 = Product.Create(
            "PROD04",
            "Milanesa Napolitana Doble",
            450,
            "Milanesa doble con jamón y queso",
            "Minutas clásicas",
            "Fritos",
            "mila2.jpg|180",
            true);

        var product3 = Product.Create(
            "PROD05",
            "Ensalada César Premium",
            300,
            "Ensalada con pollo y aderezo césar",
            "Desayunos",
            "Fritos",
            "ensalada.jpg|120",
            true);

        _context.Products.AddRange(product1, product2, product3);
        _context.SaveChanges();

        _repo.Delete(p => p.Line == "Minutas clásicas");

        var remaining = _context.Products.ToList();

        Assert.AreEqual(1, remaining.Count);
        Assert.AreEqual("PROD05", remaining[0].Code);
    }
}

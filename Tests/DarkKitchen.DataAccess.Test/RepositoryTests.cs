using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test;

[TestClass]
public class RepositoryTests
{
    private AppDbContext _context = null!;
    private Repository<Product> _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

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
            "PROD01", "Hamburguesa Clásica",
            "Hamburguesa con queso y lechuga fresca",
            "Combo burgers", "Parrilla", "hamburguesa.jpg|100", true
        );
        _context.Products.Add(product);
        _context.SaveChanges();
        
        _repository.Delete(p => p.Code == "PROD01");
        
        var remaining = _context.Products.ToList();
        Assert.AreEqual(0, remaining.Count);
    }
}

using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test;

[TestClass]
public class ProductRepositoryTests
{
    private AppDbContext _context = null!;
    private ProductRepository _repository = null!;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new ProductRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private void SeedProducts()
    {
        var products = new List<Product>
        {
            new Product
            {
                Code = "BURG01",
                Name = "Hamburguesa clasica",
                Description = "Hamburguesa con lechuga y tomate",
                Price = 250m,
                Line = "Combo burgers",
                Category = "Parrilla",
                ImageUrls = ["http://img.com/burg1.jpg"],
                Active = true,
            },
            new Product
            {
                Code = "PAST01",
                Name = "Ravioles de verdura",
                Description = "Ravioles caseros con salsa fileto",
                Price = 300m,
                Line = "Minutas clasicas",
                Category = "Pastas",
                ImageUrls = ["http://img.com/past1.jpg"],
                Active = true,
            },
            new Product
            {
                Code = "FRIT01",
                Name = "Papas fritas grandes",
                Description = "Papas fritas crocantes con sal",
                Price = 150m,
                Line = "Combo burgers",
                Category = "Fritos",
                ImageUrls = ["http://img.com/frit1.jpg"],
                Active = true,
            },
            new Product
            {
                Code = "DESAY01",
                Name = "Tostadas con mermelada",
                Description = "Tostadas de pan integral con mermelada casera",
                Price = 180m,
                Line = "Desayunos",
                Category = "Parrilla",
                ImageUrls = ["http://img.com/des1.jpg"],
                Active = false,
            },
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [TestMethod]
    public void GetFiltered_NoFilters_ReturnsAllProducts()
    {
        SeedProducts();

        var result = _repository.GetFiltered(null, null, null);

        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByLine_ReturnsMatchingProducts()
    {
        SeedProducts();

        var result = _repository.GetFiltered("Combo burgers", null, null);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Line == "Combo burgers"));
    }

    [TestMethod]
    public void GetFiltered_EmptyDatabase_ReturnsEmptyList()
    {
        var result = _repository.GetFiltered(null, null, null);

        Assert.AreEqual(0, result.Count);
    }
}

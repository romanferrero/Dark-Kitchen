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

    [TestMethod]
    public void GetFiltered_ByCategory_ReturnsMatchingProducts()
    {
        SeedProducts();

        var categories = new List<string> { "Pastas", "Fritos" };

        var result = _repository.GetFiltered(null, categories, null);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => categories.Contains(p.Category)));
    }

    [TestMethod]
    public void GetFiltered_BySingleCategory_ReturnsMatchingProducts()
    {
        SeedProducts();

        var categories = new List<string> { "Parrilla" };

        var result = _repository.GetFiltered(null, categories, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByName_ReturnsPartialMatch()
    {
        SeedProducts();

        var result = _repository.GetFiltered(null, null, "Hamburguesa");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void GetFiltered_ByNameCaseInsensitive_ReturnsMatch()
    {
        SeedProducts();

        var result = _repository.GetFiltered(null, null, "hamburguesa");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void GetFiltered_AllFiltersCombined_ReturnsMatchingProducts()
    {
        SeedProducts();

        var categories = new List<string> { "Parrilla" };

        var result = _repository.GetFiltered("Combo burgers", categories, "Hamburguesa");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void GetFiltered_NoMatches_ReturnsEmptyList()
    {
        SeedProducts();

        var result = _repository.GetFiltered("Linea inexistente", null, null);

        Assert.AreEqual(0, result.Count);
    }
}

using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.RepositoriesTests;

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
            Product.Create(
                code: "BURG01",
                name: "Hamburguesa clasica",
                price: 250,
                description: "Hamburguesa con lechuga y tomate",
                line: "Combo burgers",
                category: "Parrilla",
                images: "http://img.com/burg1.jpg|100",
                active: true),

            Product.Create(
                code: "PAST01",
                name: "Ravioles de verdura",
                price: 300,
                description: "Ravioles caseros con salsa fileto",
                line: "Minutas clasicas",
                category: "Pastas",
                images: "http://img.com/past1.jpg|100",
                active: true),

            Product.Create(
                code: "FRIT01",
                name: "Papas fritas grandes",
                price: 180,
                description: "Papas fritas crocantes con sal",
                line: "Combo burgers",
                category: "Fritos",
                images: "http://img.com/frit1.jpg|100",
                active: true),

            Product.Create(
                code: "DESAY01",
                name: "Tostadas con mermelada",
                price: 150,
                description: "Tostadas de pan integral con mermelada casera",
                line: "Desayunos",
                category: "Parrilla",
                images: "http://img.com/des1.jpg|100",
                active: false)
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [TestMethod]
    public void GetAll_ExistingProduct_ReturnsProduct()
    {
        SeedProducts();

        var result = _repository.GetAll(p => p.Code == "BURG01").FirstOrDefault();

        Assert.IsNotNull(result);
        Assert.AreEqual("Hamburguesa clasica", result.Name);
    }

    [TestMethod]
    public void GetAll_NonExistingProduct_ReturnsNull()
    {
        SeedProducts();

        var result = _repository.GetAll(p => p.Code == "NOEXISTE").FirstOrDefault();

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ExistingProduct_PersistsChanges()
    {
        SeedProducts();

        var product = _context.Products.First(p => p.Code == "BURG01");

        product.Update(
            name: "Hamburguesa especial",
            price: 500,
            description: "Hamburguesa con doble carne y queso cheddar",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg|100",
            active: true);

        _repository.Update(product);

        var updated = _context.Products.First(p => p.Code == "BURG01");

        Assert.AreEqual("Hamburguesa especial", updated.Name);
        Assert.AreEqual(500, updated.Price);
        Assert.AreEqual("Hamburguesa con doble carne y queso cheddar", updated.Description);
        Assert.IsTrue(updated.Active);
    }

    [TestMethod]
    public void Add_ValidProduct_PersistsInDatabase()
    {
        var product = Product.Create(
            code: "BURG01",
            name: "Hamburguesa clasica",
            price: 250,
            description: "Hamburguesa con lechuga y tomate",
            line: "Combo burgers",
            category: "Parrilla",
            images: "http://img.com/burg1.jpg|100",
            active: true);

        _repository.Add(product);

        var saved = _context.Products.FirstOrDefault(p => p.Code == "BURG01");

        Assert.IsNotNull(saved);
        Assert.AreEqual("Hamburguesa clasica", saved.Name);
    }

    [TestMethod]
    public void GetFiltered_NoPredicate_ReturnsAllProducts()
    {
        SeedProducts();

        var result = _repository.GetFiltered();

        Assert.AreEqual(4, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByLine_ReturnsMatchingProducts()
    {
        SeedProducts();

        var result = _repository.GetFiltered(p => p.Line == "Combo burgers");

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => p.Line == "Combo burgers"));
    }

    [TestMethod]
    public void GetFiltered_EmptyDatabase_ReturnsEmptyList()
    {
        var result = _repository.GetFiltered();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByCategory_ReturnsMatchingProducts()
    {
        SeedProducts();

        var categories = new List<string> { "Pastas", "Fritos" };

        var result = _repository.GetFiltered(p => categories.Contains(p.Category));

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.All(p => categories.Contains(p.Category)));
    }

    [TestMethod]
    public void GetFiltered_ByName_ReturnsPartialMatch()
    {
        SeedProducts();

        var result = _repository.GetFiltered(p => p.Name.Contains("Hamburguesa"));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void GetFiltered_AllFiltersCombined_ReturnsMatchingProducts()
    {
        SeedProducts();

        var categories = new List<string> { "Parrilla" };

        var result = _repository.GetFiltered(p =>
            p.Line == "Combo burgers" &&
            categories.Contains(p.Category) &&
            p.Name.Contains("Hamburguesa"));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("BURG01", result[0].Code);
    }

    [TestMethod]
    public void GetFiltered_NoMatches_ReturnsEmptyList()
    {
        SeedProducts();

        var result = _repository.GetFiltered(p => p.Line == "Linea inexistente");

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetFiltered_IncludesImages()
    {
        SeedProducts();

        var result = _repository.GetFiltered(p => p.Code == "BURG01");

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result[0].Images.Count > 0);
    }
}

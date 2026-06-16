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
            Product.Create(new CreateProductParamsDto(
                Code: "BURG01",
                Name: "Hamburguesa clasica",
                Price: 250,
                Description: "Hamburguesa con lechuga y tomate",
                Line: "Combo burgers",
                Category: "Parrilla",
                Images: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==",
                Active: true)),

            Product.Create(new CreateProductParamsDto(
                Code: "PAST01",
                Name: "Ravioles de verdura",
                Price: 300,
                Description: "Ravioles caseros con salsa fileto",
                Line: "Minutas clasicas",
                Category: "Pastas",
                Images: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==",
                Active: true)),

            Product.Create(new CreateProductParamsDto(
                Code: "FRIT01",
                Name: "Papas fritas grandes",
                Price: 180,
                Description: "Papas fritas crocantes con sal",
                Line: "Combo burgers",
                Category: "Fritos",
                Images: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==",
                Active: true)),

            Product.Create(new CreateProductParamsDto(
                Code: "DESAY01",
                Name: "Tostadas con mermelada",
                Price: 150,
                Description: "Tostadas de pan integral con mermelada casera",
                Line: "Desayunos",
                Category: "Parrilla",
                Images: "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ==",
                Active: false))
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
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

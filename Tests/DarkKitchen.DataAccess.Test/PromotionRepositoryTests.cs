using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test;

[TestClass]
public class PromotionRepositoryTests
{
    private AppDbContext _context = null!;
    private PromotionRepository _repository = null!;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new PromotionRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ValidPromotion_PersistsInDatabase()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));

        _repository.Add(promotion);

        var saved = _context.Promotions.FirstOrDefault(p => p.Name == "Black Friday");
        Assert.IsNotNull(saved);
        Assert.AreEqual(10, saved.DiscountPercentage);
    }

    [TestMethod]
    public void GetAll_ExistingPromotion_ReturnsPromotion()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        _repository.Add(promotion);

        var result = _repository.GetAll(p => p.Id == promotion.Id).FirstOrDefault();

        Assert.IsNotNull(result);
        Assert.AreEqual("Black Friday", result.Name);
    }

    [TestMethod]
    public void Update_ExistingPromotion_PersistsChanges()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        _repository.Add(promotion);

        promotion.Update("Cyber Monday", 25, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 7));
        _repository.Update(promotion);

        var updated = _context.Promotions.First(p => p.Id == promotion.Id);
        Assert.AreEqual("Cyber Monday", updated.Name);
        Assert.AreEqual(25, updated.DiscountPercentage);
    }

    [TestMethod]
    public void GetAll_NonExistingPromotion_ReturnsEmpty()
    {
        var result = _repository.GetAll(p => p.Id == 999);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetAll_NoPredicate_ReturnsAllPromotions()
    {
        _context.Promotions.AddRange(
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)),
            Promotion.Create("Cyber Monday", 20, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 7)));
        _context.SaveChanges();

        var result = _repository.GetAll();

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByDate_ReturnsActivePromotions()
    {
        _context.Promotions.AddRange(
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)),
            Promotion.Create("Semana de Turismo", 15, new DateOnly(2026, 3, 29), new DateOnly(2026, 4, 4)));
        _context.SaveChanges();

        var result = _repository.GetFiltered(new DateOnly(2026, 5, 15), null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Black Friday", result[0].Name);
    }

    [TestMethod]
    public void GetFiltered_NoFilters_ReturnsAllPromotions()
    {
        _context.Promotions.AddRange(
            Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31)),
            Promotion.Create("Semana de Turismo", 15, new DateOnly(2026, 3, 29), new DateOnly(2026, 4, 4)));
        _context.SaveChanges();

        var result = _repository.GetFiltered(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByLine_ReturnsMatchingPromotions()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var other = Promotion.Create("Semana de Turismo", 15, new DateOnly(2026, 3, 29), new DateOnly(2026, 4, 4));

        var product = Product.Create("BURG01", "Hamburguesa clasica", "Hamburguesa con lechuga y tomate fresco", "Combo burgers", "Parrilla", "http://img.com/b.jpg", true);
        promotion.AddProduct(product);

        _context.Promotions.AddRange(promotion, other);
        _context.SaveChanges();

        var result = _repository.GetFiltered(null, "Combo burgers", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Black Friday", result[0].Name);
    }

    [TestMethod]
    public void GetFiltered_ByProduct_ReturnsMatchingPromotions()
    {
        var promotion = Promotion.Create("Black Friday", 10, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 31));
        var other = Promotion.Create("Semana de Turismo", 15, new DateOnly(2026, 3, 29), new DateOnly(2026, 4, 4));

        var product = Product.Create("BURG01", "Hamburguesa clasica", "Hamburguesa con lechuga y tomate fresco", "Combo burgers", "Parrilla", "http://img.com/b.jpg", true);
        promotion.AddProduct(product);

        _context.Promotions.AddRange(promotion, other);
        _context.SaveChanges();

        var result = _repository.GetFiltered(null, null, "BURG01");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Black Friday", result[0].Name);
    }
}

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
}

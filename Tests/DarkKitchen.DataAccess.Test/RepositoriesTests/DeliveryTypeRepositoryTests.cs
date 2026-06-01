using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.RepositoriesTests;

[TestClass]
public class DeliveryTypeRepositoryTests
{
    private AppDbContext _context = null!;
    private Repository<DeliveryType> _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();
        _repository = new Repository<DeliveryType>(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [TestMethod]
    public void Add_ValidDeliveryType_PersistsToDatabase()
    {
        var deliveryType = DeliveryType.Create("Express", 250m);

        _repository.Add(deliveryType);

        var stored = _context.Set<DeliveryType>().First(d => d.Name == "Express");
        Assert.AreEqual(250m, stored.ShippingCost);
    }
}

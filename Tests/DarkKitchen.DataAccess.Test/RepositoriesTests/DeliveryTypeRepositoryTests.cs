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
        var deliveryType = DeliveryType.Create("SuperExpress", 300m);

        _repository.Add(deliveryType);

        var stored = _context.Set<DeliveryType>().First(d => d.Name == "SuperExpress");
        Assert.AreEqual(300m, stored.ShippingCost);
    }

    [TestMethod]
    public void Get_ByName_ReturnsCorrectEntry()
    {
        _repository.Add(DeliveryType.Create("SuperExpress", 300m));

        var result = _repository.Get(d => d.Name == "SuperExpress");

        Assert.IsNotNull(result);
        Assert.AreEqual(300m, result.ShippingCost);
    }

    [TestMethod]
    public void Update_ChangesNameAndCost()
    {
        var deliveryType = DeliveryType.Create("SuperExpress", 300m);
        _repository.Add(deliveryType);

        deliveryType.Update("EcoDelivery", 100m);
        _repository.Update(deliveryType);

        var stored = _context.Set<DeliveryType>().First(d => d.Id == deliveryType.Id);
        Assert.AreEqual("EcoDelivery", stored.Name);
        Assert.AreEqual(100m, stored.ShippingCost);
    }
}

using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.DataAccess.Test.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.RepositoriesTests;

[TestClass]
public class RepositoryTests
{
    private TestDbContext _context = null!;
    private Repository<TestEntity> _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repo = new Repository<TestEntity>(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    private void Seed(params string[] names)
    {
        foreach(var name in names)
        {
            _context.Entities.Add(new TestEntity { Name = name });
        }

        _context.SaveChanges();
    }

    [TestMethod]
    public void Add_PersistsEntity()
    {
        _repo.Add(new TestEntity { Name = "alpha" });

        var saved = _context.Entities.SingleOrDefault();

        Assert.IsNotNull(saved);
        Assert.AreEqual("alpha", saved.Name);
    }

    [TestMethod]
    public void Update_PersistsChanges()
    {
        Seed("alpha");
        var stored = _context.Entities.First();

        stored.Name = "alpha-updated";
        _repo.Update(stored);

        Assert.AreEqual("alpha-updated", _context.Entities.First().Name);
    }

    [TestMethod]
    public void Delete_WithMatchingPredicate_RemovesEntity()
    {
        Seed("alpha", "beta");

        _repo.Delete(e => e.Name == "alpha");

        var remaining = _context.Entities.ToList();

        Assert.AreEqual(1, remaining.Count);
        Assert.AreEqual("beta", remaining[0].Name);
    }

    [TestMethod]
    public void Delete_WithoutMatch_DoesNotRemoveAnything()
    {
        Seed("alpha");

        _repo.Delete(e => e.Name == "NOEXISTE");

        var remaining = _context.Entities.ToList();

        Assert.AreEqual(1, remaining.Count);
        Assert.AreEqual("alpha", remaining[0].Name);
    }

    [TestMethod]
    public void Delete_WithMultipleMatches_RemovesAllMatching()
    {
        Seed("alpha", "alpha", "beta");

        _repo.Delete(e => e.Name == "alpha");

        var remaining = _context.Entities.ToList();

        Assert.AreEqual(1, remaining.Count);
        Assert.AreEqual("beta", remaining[0].Name);
    }

    [TestMethod]
    public void GetAll_WithoutPredicate_ReturnsAll()
    {
        Seed("alpha", "beta", "gamma");

        var result = _repo.GetAll();

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public void GetAll_WithPredicate_ReturnsMatching()
    {
        Seed("alpha", "beta", "alpha2");

        var result = _repo.GetAll(e => e.Name.StartsWith("alpha"));

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var result = _repo.GetAll();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Get_WithMatch_ReturnsEntity()
    {
        Seed("alpha", "beta");

        var result = _repo.Get(e => e.Name == "alpha");

        Assert.IsNotNull(result);
        Assert.AreEqual("alpha", result.Name);
    }

    [TestMethod]
    public void Get_WithoutMatch_ReturnsNull()
    {
        Seed("alpha");

        var result = _repo.Get(e => e.Name == "NOEXISTE");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Exists_WithMatch_ReturnsTrue()
    {
        Seed("alpha");

        Assert.IsTrue(_repo.Exists(e => e.Name == "alpha"));
    }

    [TestMethod]
    public void Exists_WithoutMatch_ReturnsFalse()
    {
        Seed("alpha");

        Assert.IsFalse(_repo.Exists(e => e.Name == "NOEXISTE"));
    }
}

using DarkKitchen.DataAccess.Context;
using DarkKitchen.DataAccess.Repositories;
using DarkKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Test.RepositoriesTests;

[TestClass]
public class AuditLogRepositoryTests
{
    private AppDbContext _context = null!;
    private AuditLogRepository _repository = null!;

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _repository = new AuditLogRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }

    private AuditLog CreateLog(
        string entityName = "PRODUCT",
        int entityId = 1,
        string description = "Creation",
        string responsibleUser = "admin@darkkitchen.com")
        => AuditLog.Create(entityName, entityId, description, responsibleUser);

    [TestMethod]
    public void GetFiltered_NoParameters_ReturnsAllLogs()
    {
        _context.AuditLogs.Add(CreateLog(entityId: 1));
        _context.AuditLogs.Add(CreateLog(entityId: 2));
        _context.SaveChanges();

        var result = _repository.GetFiltered(DateTime.MinValue, DateTime.MaxValue);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void GetFiltered_DateRangeExcludesAllLogs_ReturnsEmpty()
    {
        _context.AuditLogs.Add(CreateLog(entityId: 1));
        _context.AuditLogs.Add(CreateLog(entityId: 2));
        _context.SaveChanges();

        var futureFrom = DateTime.UtcNow.AddHours(1);
        var result = _repository.GetFiltered(futureFrom, DateTime.MaxValue);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetFiltered_ByEntityName_ReturnsMatchingLogs()
    {
        _context.AuditLogs.Add(CreateLog(entityName: "PRODUCTO", entityId: 1));
        _context.AuditLogs.Add(CreateLog(entityName: "PROMOCION", entityId: 2));
        _context.SaveChanges();

        var result = _repository.GetFiltered(DateTime.MinValue, DateTime.MaxValue, entityName: "PRODUCTO");

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("PRODUCTO", result[0].EntityName);
    }

    [TestMethod]
    public void GetFiltered_ByEntityId_ReturnsMatchingLogs()
    {
        _context.AuditLogs.Add(CreateLog(entityId: 10));
        _context.AuditLogs.Add(CreateLog(entityId: 20));
        _context.SaveChanges();

        var result = _repository.GetFiltered(DateTime.MinValue, DateTime.MaxValue, entityId: 10);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(10, result[0].EntityId);
    }

    [TestMethod]
    public void GetFiltered_AllFilters_ReturnsOnlyMatchingLog()
    {
        _context.AuditLogs.Add(CreateLog(entityName: "PRODUCTO", entityId: 5));
        _context.AuditLogs.Add(CreateLog(entityName: "PRODUCTO", entityId: 99));
        _context.AuditLogs.Add(CreateLog(entityName: "PROMOCION", entityId: 5));
        _context.SaveChanges();

        var result = _repository.GetFiltered(DateTime.MinValue, DateTime.MaxValue, "PRODUCTO", 5);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("PRODUCTO", result[0].EntityName);
        Assert.AreEqual(5, result[0].EntityId);
    }
}

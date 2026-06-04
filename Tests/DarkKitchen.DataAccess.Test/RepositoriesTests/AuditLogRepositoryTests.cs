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
        string entityName = "PRODUCTO",
        int entityId = 1,
        string description = "Creación",
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
}

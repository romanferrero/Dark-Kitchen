using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

[TestClass]
public class AuditLogServiceTests
{
    private Mock<IAuditLogRepository> _auditRepoMock = null!;
    private AuditLogService _auditService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _auditRepoMock = new Mock<IAuditLogRepository>(MockBehavior.Strict);
        _auditService = new AuditLogService(_auditRepoMock.Object);
    }

    [TestMethod]
    public void GetAuditLogs_DateFromEqualDateTo_ThrowsArgumentException()
    {
        var date = new DateTime(2026, 4, 23, 8, 0, 0);

        Assert.ThrowsException<ArgumentException>(() =>
            _auditService.GetAuditLogs(date, date));
    }

    [TestMethod]
    public void GetAuditLogs_DateFromAfterDateTo_ThrowsArgumentException()
    {
        var dateFrom = new DateTime(2026, 4, 23, 10, 0, 0);
        var dateTo = new DateTime(2026, 4, 23, 8, 0, 0);

        Assert.ThrowsException<ArgumentException>(() =>
            _auditService.GetAuditLogs(dateFrom, dateTo));
    }

    [TestMethod]
    public void GetAuditLogs_ValidRange_DelegatesToRepositoryAndMapsResult()
    {
        var dateFrom = new DateTime(2026, 4, 23, 8, 0, 0);
        var dateTo = new DateTime(2026, 4, 23, 10, 0, 0);
        var log = AuditLog.Create("PRODUCTO", 12345, "Creación", "admin@darkkitchen.com");

        _auditRepoMock
            .Setup(r => r.GetFiltered(dateFrom, dateTo, null, null))
            .Returns([log]);

        var result = _auditService.GetAuditLogs(dateFrom, dateTo);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("PRODUCTO", result[0].EntityName);
        Assert.AreEqual(12345, result[0].EntityId);
        Assert.AreEqual("Creación", result[0].Description);
        Assert.AreEqual("admin@darkkitchen.com", result[0].ResponsibleUser);
    }

    [TestMethod]
    public void GetAuditLogs_WithEntityFilters_PassesFiltersToRepository()
    {
        var dateFrom = new DateTime(2026, 4, 23, 8, 0, 0);
        var dateTo = new DateTime(2026, 4, 23, 10, 0, 0);

        _auditRepoMock
            .Setup(r => r.GetFiltered(dateFrom, dateTo, "PRODUCTO", 12345))
            .Returns([]);

        var result = _auditService.GetAuditLogs(dateFrom, dateTo, "PRODUCTO", 12345);

        _auditRepoMock.Verify(r => r.GetFiltered(dateFrom, dateTo, "PRODUCTO", 12345), Times.Once);
        Assert.AreEqual(0, result.Count);
    }
}

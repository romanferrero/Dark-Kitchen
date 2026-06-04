using DarkKitchen.BusinessLogic.Services;
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
}

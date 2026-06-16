using DarkKitchen.IBusinessLogic.DTOs.Exit;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models.Request.AuditModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.AuditControllersTests;

[TestClass]
public class AuditControllerTests
{
    private Mock<IAuditLogService> _auditServiceMock = null!;
    private AuditController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _auditServiceMock = new Mock<IAuditLogService>(MockBehavior.Strict);
        _controller = new AuditController(_auditServiceMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [TestMethod]
    public void GetAuditLogs_ValidFilters_Returns200WithList()
    {
        var query = new AuditLogQueryModel
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0),
        };

        _auditServiceMock
            .Setup(s => s.GetAuditLogs(query.DateFrom.Value, query.DateTo.Value, null, null))
            .Returns([new AuditLogExitDto(0, default, "PRODUCTO", 1, default!, default!)]);

        var result = _controller.GetAuditLogs(query) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetAuditLogs_MissingDateFrom_ThrowsArgumentException()
    {
        var query = new AuditLogQueryModel
        {
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0),
        };

        _auditServiceMock
            .Setup(s => s.GetAuditLogs(null, query.DateTo, null, null))
            .Throws(new ArgumentException("DateFrom and DateTo are required."));

        Assert.ThrowsException<ArgumentException>(() => _controller.GetAuditLogs(query));
    }

    [TestMethod]
    public void GetAuditLogs_MissingDateTo_ThrowsArgumentException()
    {
        var query = new AuditLogQueryModel
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
        };

        _auditServiceMock
            .Setup(s => s.GetAuditLogs(query.DateFrom, null, null, null))
            .Throws(new ArgumentException("DateFrom and DateTo are required."));

        Assert.ThrowsException<ArgumentException>(() => _controller.GetAuditLogs(query));
    }

    [TestMethod]
    public void GetAuditLogs_InvalidDateRange_ThrowsArgumentException()
    {
        var query = new AuditLogQueryModel
        {
            DateFrom = new DateTime(2026, 4, 23, 10, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 8, 0, 0),
        };

        _auditServiceMock
            .Setup(s => s.GetAuditLogs(query.DateFrom.Value, query.DateTo.Value, null, null))
            .Throws(new ArgumentException("DateFrom must be earlier than DateTo."));

        Assert.ThrowsException<ArgumentException>(() => _controller.GetAuditLogs(query));
    }

    [TestMethod]
    public void GetAuditLogs_WithEntityFilters_PassesFiltersToService()
    {
        var query = new AuditLogQueryModel
        {
            DateFrom = new DateTime(2026, 4, 23, 8, 0, 0),
            DateTo = new DateTime(2026, 4, 23, 10, 0, 0),
            EntityName = "PRODUCTO",
            EntityId = 12345,
        };

        _auditServiceMock
            .Setup(s => s.GetAuditLogs(query.DateFrom.Value, query.DateTo.Value, "PRODUCTO", 12345))
            .Returns([]);

        var result = _controller.GetAuditLogs(query) as OkObjectResult;

        Assert.IsNotNull(result);
        _auditServiceMock.Verify(
            s => s.GetAuditLogs(query.DateFrom.Value, query.DateTo.Value, "PRODUCTO", 12345),
            Times.Once);
    }
}

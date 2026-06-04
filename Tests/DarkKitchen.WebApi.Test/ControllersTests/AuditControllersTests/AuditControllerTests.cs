using DarkKitchen.IBusinessLogic.DTOs.Exit.AuditLogDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.AuditControllers;
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
            .Returns([new AuditLogExitDto { EntityName = "PRODUCTO", EntityId = 1 }]);

        var result = _controller.GetAuditLogs(query) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }
}

using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class AdminReportsControllerTests
{
    private Mock<IReportService> _reportServiceMock = null!;
    private AdminReportsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _reportServiceMock = new Mock<IReportService>();
        _controller = new AdminReportsController(_reportServiceMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_ValidDateRange_Returns200()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 12, 31);

        var result = _controller.GetTopProducts(dateFrom, dateTo);

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }
}

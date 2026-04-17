using DarkKitchen.Domain.DTOs;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Filters;
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

    [TestMethod]
    public void GetTopProducts_ValidDateRange_CallsServiceWithSameDates()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 12, 31);

        _controller.GetTopProducts(dateFrom, dateTo);

        _reportServiceMock.Verify(s => s.GetTopProducts(dateFrom, dateTo), Times.Once);
    }

    [TestMethod]
    public void GetTopProducts_HasHttpGetAttributeWithRoute()
    {
        var method = typeof(AdminReportsController).GetMethod("GetTopProducts");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("top-products", attribute.Template);
    }

    [TestMethod]
    public void GetTopProducts_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminReportsController).GetMethod("GetTopProducts");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }
}

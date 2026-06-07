using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.AdminControllers;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.AdminControllersTests;

[TestClass]
public class AdminReportsControllerTests
{
    private static readonly DateTime DateFrom = new(2026, 1, 1);
    private static readonly DateTime DateTo = new(2026, 12, 31);

    private Mock<IReportService> _reportServiceMock = null!;
    private AdminReportsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _reportServiceMock = new Mock<IReportService>(MockBehavior.Strict);
        _controller = new AdminReportsController(_reportServiceMock.Object);
    }

    [TestMethod]
    public void GetReport_TopProductsWithValidDateRange_Returns200()
    {
        _reportServiceMock
            .Setup(s => s.GetTopProducts(DateFrom, DateTo))
            .Returns([]);

        var result = _controller.GetReport("top-products", DateFrom, DateTo) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetReport_TopProductsWithValidDateRange_CallsServiceWithSameDates()
    {
        _reportServiceMock
            .Setup(s => s.GetTopProducts(DateFrom, DateTo))
            .Returns([]);

        _controller.GetReport("top-products", DateFrom, DateTo);

        _reportServiceMock.Verify(s => s.GetTopProducts(DateFrom, DateTo), Times.Once);
    }

    [TestMethod]
    public void GetReport_TopProductsWithoutDates_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _controller.GetReport("top-products", null, null));
    }

    [TestMethod]
    public void GetReport_Sales_Returns200()
    {
        _reportServiceMock
            .Setup(s => s.GetSalesReport())
            .Returns(new SalesReportExitDto());

        var result = _controller.GetReport("sales") as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetReport_Sales_CallsService()
    {
        _reportServiceMock
            .Setup(s => s.GetSalesReport())
            .Returns(new SalesReportExitDto());

        _controller.GetReport("sales");

        _reportServiceMock.Verify(s => s.GetSalesReport(), Times.Once);
    }

    [TestMethod]
    public void GetReport_UnknownType_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _controller.GetReport("unknown-type"));
    }

    [TestMethod]
    public void GetReport_HasHttpGetAttribute()
    {
        var method = typeof(AdminReportsController).GetMethod("GetReport");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.IsNull(attribute.Template);
    }

    [TestMethod]
    public void GetReport_TopProductsWithDateFromButNoDateTo_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            _controller.GetReport("top-products", DateFrom, null));
    }

    [TestMethod]
    public void GetReport_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminReportsController).GetMethod("GetReport");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }
}

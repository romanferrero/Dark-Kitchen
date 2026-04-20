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
        _reportServiceMock = new Mock<IReportService>();
        _controller = new AdminReportsController(_reportServiceMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_ValidDateRange_Returns200()
    {
        _reportServiceMock
            .Setup(s => s.GetTopProducts(DateFrom, DateTo))
            .Returns([]);

        var result = _controller.GetTopProducts(DateFrom, DateTo) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetTopProducts_ValidDateRange_CallsServiceWithSameDates()
    {
        _controller.GetTopProducts(DateFrom, DateTo);

        _reportServiceMock.Verify(s => s.GetTopProducts(DateFrom, DateTo), Times.Once);
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

    [TestMethod]
    public void GetSalesReport_Returns200()
    {
        _reportServiceMock
            .Setup(s => s.GetSalesReport())
            .Returns(new SalesReportExitDTO());

        var result = _controller.GetSalesReport() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void GetSalesReport_CallsService()
    {
        _controller.GetSalesReport();

        _reportServiceMock.Verify(s => s.GetSalesReport(), Times.Once);
    }

    [TestMethod]
    public void GetSalesReport_HasHttpGetAttributeWithRoute()
    {
        var method = typeof(AdminReportsController).GetMethod("GetSalesReport");

        var attribute = method!
            .GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("sales", attribute.Template);
    }

    [TestMethod]
    public void GetSalesReport_HasAuthorizationFilterForAdmin()
    {
        var method = typeof(AdminReportsController).GetMethod("GetSalesReport");

        var attributes = method!.GetCustomAttributes(typeof(AuthorizationFilter), false);

        Assert.AreEqual(1, attributes.Length);
    }
}

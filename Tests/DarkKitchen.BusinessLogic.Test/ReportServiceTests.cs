using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.Domain.DTOs;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Tests;

[TestClass]
public class ReportServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IRepository<User>> _userRepositoryMock = null!;
    private ReportService _reportService = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _userRepositoryMock = new Mock<IRepository<User>>();
        _reportService = new ReportService(_orderRepositoryMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_NoOrdersInRange_ReturnsEmptyList()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);

        _orderRepositoryMock
            .Setup(r => r.GetTopSellingProducts(dateFrom, dateTo, 5))
            .Returns(new List<TopProductDto>());

        var result = _reportService.GetTopProducts(dateFrom, dateTo);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopProducts_WithOrders_ReturnsTopProductsFromRepository()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 3, 31);

        var expectedProducts = new List<TopProductDto>
        {
            new()
            {
                Code = "PROD01",
                Name = "Hamburguesa Clásica",
                QuantitySold = 10,
                ImageUrls = ["http://img.com/burger.jpg"]
            },
            new()
            {
                Code = "PROD02",
                Name = "Pizza Muzzarella Grande",
                QuantitySold = 7,
                ImageUrls = ["http://img.com/pizza.jpg"]
            }
        };

        _orderRepositoryMock
            .Setup(r => r.GetTopSellingProducts(dateFrom, dateTo, 5))
            .Returns(expectedProducts);

        var result = _reportService.GetTopProducts(dateFrom, dateTo);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("PROD01", result[0].Code);
        Assert.AreEqual(10, result[0].QuantitySold);
        Assert.AreEqual("PROD02", result[1].Code);
        Assert.AreEqual(7, result[1].QuantitySold);
    }

    [TestMethod]
    public void GetTopProducts_CallsRepositoryWithCorrectParameters()
    {
        var dateFrom = new DateTime(2026, 2, 1);
        var dateTo = new DateTime(2026, 2, 28);

        _orderRepositoryMock
            .Setup(r => r.GetTopSellingProducts(dateFrom, dateTo, 5))
            .Returns(new List<TopProductDto>());

        _reportService.GetTopProducts(dateFrom, dateTo);

        _orderRepositoryMock.Verify(
            r => r.GetTopSellingProducts(dateFrom, dateTo, 5),
            Times.Once);
    }

    [TestMethod]
    public void GetSalesReport_NoOrders_ReturnsEmptyReport()
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<User>());

        _orderRepositoryMock
            .Setup(r => r.GetMonthlySalesGroupedByClient(It.IsAny<List<User>>()))
            .Returns(new List<MonthlySalesDto>());

        var result = _reportService.GetSalesReport();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.MonthlySales.Count);
        Assert.AreEqual(0m, result.GrandTotal);
    }
}

using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.SalesDTOs;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;
using Moq;

namespace DarkKitchen.BusinessLogic.Test.ServicesTests;

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
        _reportService = new ReportService(_orderRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_NoOrdersInRange_ReturnsEmptyList()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 1, 31);

        SetupTopProducts(dateFrom, dateTo, []);

        var result = _reportService.GetTopProducts(dateFrom, dateTo);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopProducts_WithOrders_ReturnsTopProductsFromRepository()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 3, 31);

        var expectedProducts = new List<TopProductExitDTO>
        {
            CreateTopProduct("PROD01", "Hamburguesa Clásica", 10, "http://img.com/burger.jpg"),
            CreateTopProduct("PROD02", "Pizza Muzzarella Grande", 7, "http://img.com/pizza.jpg")
        };

        SetupTopProducts(dateFrom, dateTo, expectedProducts);

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

        SetupTopProducts(dateFrom, dateTo, []);

        _reportService.GetTopProducts(dateFrom, dateTo);

        _orderRepositoryMock.Verify(
            r => r.GetTopSellingProducts(dateFrom, dateTo, 5),
            Times.Once);
    }

    [TestMethod]
    public void GetSalesReport_NoOrders_ReturnsEmptyReport()
    {
        SetupUsers();
        SetupMonthlySales();

        var result = _reportService.GetSalesReport();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.MonthlySales.Count);
        Assert.AreEqual(0m, result.GrandTotal);
    }

    [TestMethod]
    public void GetSalesReport_WithOrders_CalculatesGrandTotalCorrectly()
    {
        SetupUsers();

        var monthlySales = new List<MonthlySalesExitDTO>
        {
            CreateMonthlySales(
                "2026-01",
                9000m,
                new ClientSalesExitDTO { ClientName = "Juan Perez", Total = 5000m },
                new ClientSalesExitDTO { ClientName = "Yuri Gagarin", Total = 4000m }),
            CreateMonthlySales(
                "2026-02",
                6600m,
                new ClientSalesExitDTO { ClientName = "Sommer Schutman", Total = 5600m },
                new ClientSalesExitDTO { ClientName = "Juan Perez", Total = 1000m })
        };

        SetupMonthlySales(monthlySales);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(15600m, result.GrandTotal);
    }

    [TestMethod]
    public void GetSalesReport_WithOrders_ReturnsCorrectMonthlyStructure()
    {
        SetupUsers();

        var monthlySales = new List<MonthlySalesExitDTO>
        {
            CreateMonthlySales(
                "2026-01",
                9000m,
                new ClientSalesExitDTO { ClientName = "Juan Perez", Total = 5000m },
                new ClientSalesExitDTO { ClientName = "Yuri Gagarin", Total = 4000m })
        };

        SetupMonthlySales(monthlySales);

        var result = _reportService.GetSalesReport();

        Assert.AreEqual(1, result.MonthlySales.Count);
        var firstMonth = result.MonthlySales[0];
        Assert.AreEqual("2026-01", firstMonth.Period);
        Assert.AreEqual(9000m, firstMonth.MonthlyTotal);
        Assert.AreEqual(2, firstMonth.ClientSales.Count);
        Assert.AreEqual("Juan Perez", firstMonth.ClientSales[0].ClientName);
        Assert.AreEqual(5000m, firstMonth.ClientSales[0].Total);
    }

    [TestMethod]
    public void GetSalesReport_PassesUsersToRepository()
    {
        var users = new List<User>
        {
            User.CreateClient("Juan", "Perez", "juan@test.com", "099123456", "Passw0rd!abcdefg")
        };

        SetupUsers(users);
        SetupMonthlySales();

        _reportService.GetSalesReport();

        _orderRepositoryMock.Verify(
            r => r.GetMonthlySalesGroupedByClient(
                It.Is<List<User>>(u => u.Count == 1 && u[0].FirstName == "Juan")),
            Times.Once);
    }

    private void SetupUsers(List<User>? users = null)
    {
        _userRepositoryMock
            .Setup(r => r.GetAll(null))
            .Returns(users ?? []);
    }

    private void SetupMonthlySales(List<MonthlySalesExitDTO>? monthlySales = null)
    {
        _orderRepositoryMock
            .Setup(r => r.GetMonthlySalesGroupedByClient(It.IsAny<List<User>>()))
            .Returns(monthlySales ?? []);
    }

    private void SetupTopProducts(DateTime dateFrom, DateTime dateTo, List<TopProductExitDTO> products)
    {
        _orderRepositoryMock
            .Setup(r => r.GetTopSellingProducts(dateFrom, dateTo, 5))
            .Returns(products);
    }

    private static MonthlySalesExitDTO CreateMonthlySales(
        string period,
        decimal monthlyTotal,
        params ClientSalesExitDTO[] clientSales)
    {
        return new MonthlySalesExitDTO { Period = period, MonthlyTotal = monthlyTotal, ClientSales = clientSales.ToList() };
    }

    private static TopProductExitDTO CreateTopProduct(
        string code,
        string name,
        int quantitySold,
        string imageUrl)
    {
        return new TopProductExitDTO
        {
            Code = code,
            Name = name,
            QuantitySold = quantitySold,
            ImageUrls = [imageUrl]
        };
    }
}

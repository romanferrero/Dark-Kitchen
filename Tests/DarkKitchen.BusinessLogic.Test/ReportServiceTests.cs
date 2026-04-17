using DarkKitchen.BusinessLogic.Services;
using DarkKitchen.Domain;
using DarkKitchen.IDataAccess;
using Moq;

namespace DarkKitchen.BusinessLogic.Test;

[TestClass]
public class ReportServiceTests
{
    private Mock<IRepository<Order>> _orderRepoMock = null!;
    private Mock<IRepository<User>> _userRepoMock = null!;
    private ReportService _reportService = null!;

    [TestInitialize]
    public void Setup()
    {
        _orderRepoMock = new Mock<IRepository<Order>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _reportService = new ReportService(_orderRepoMock.Object, _userRepoMock.Object);
    }

    [TestMethod]
    public void GetTopProducts_NoOrders_ReturnsEmptyList()
    {
        _orderRepoMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<Order>());

        var result = _reportService.GetTopProducts(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31));

        Assert.AreEqual(0, result.Count);
    }
}

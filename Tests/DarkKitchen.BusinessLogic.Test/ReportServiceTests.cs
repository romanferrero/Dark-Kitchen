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

    [TestMethod]
    public void GetTopProducts_WithOrders_ReturnsProductsOrderedByQuantity()
    {
        var productA = Product.Create("PROD01", "Producto AAA uno", "Descripcion larga del producto A", "Linea1",
            "Cat1", "http://img.com/a.jpg", true);
        var productB = Product.Create("PROD02", "Producto BBB dos", "Descripcion larga del producto B", "Linea1",
            "Cat1", "http://img.com/b.jpg", true);

        var address = Address.Create("Calle", "123", "Apt");

        var order1 = Order.Create(1, DeliveryType.Express, address, new List<Product> { productA, productB }, 1, 1, 100,
            10, 110);
        var order2 = Order.Create(2, DeliveryType.Express, address, new List<Product> { productA }, 1, 2, 50, 10, 60);

        _orderRepoMock
            .Setup(r => r.GetAll(null))
            .Returns(new List<Order> { order1, order2 });

        var result = _reportService.GetTopProducts(
            DateTime.Now.AddDays(-1),
            DateTime.Now.AddDays(1));

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("PROD01", result[0].Code);
        Assert.AreEqual(2, result[0].QuantitySold);
        Assert.AreEqual("PROD02", result[1].Code);
        Assert.AreEqual(1, result[1].QuantitySold);
    }
}

using DarkKitchen.IBusinessLogic.DTOs.Exit.OrderDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Controllers.OrdersControllers;
using DarkKitchen.WebApi.Models;
using DarkKitchen.WebApi.Models.Request.OrdersModels;
using DarkKitchen.WebApi.Models.Response.OrdersModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests.OrdersControllersTests;

[TestClass]
public class OrderQueriesControllerTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private OrdersController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        _controller = new OrdersController(_orderServiceMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _controller.HttpContext.Items["UserId"] = 1;
        _controller.HttpContext.Items["UserRole"] = "Client";
    }

    [TestMethod]
    public void GetDispatcherOrders_MissingDateRange_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(
            () => _controller.GetDispatcherOrders(new GetOrdersQueryModel()));
    }

    [TestMethod]
    public void GetClientOrders_UsesClientIdFromToken()
    {
        _controller.HttpContext.Items["UserId"] = 42;

        _orderServiceMock
            .Setup(s => s.GetClientOrders(42, null, null, null))
            .Returns([]);

        _controller.GetClientOrders(new GetOrdersQueryModel());

        _orderServiceMock.Verify(s => s.GetClientOrders(42, null, null, null), Times.Once);
    }

    [TestMethod]
    public void GetDispatcherOrders_ValidDateRange_Returns200WithList()
    {
        var from = DateTime.Today.AddDays(-7);
        var to = DateTime.Today;

        _orderServiceMock
            .Setup(s => s.GetDispatcherOrders(from, to, null, null))
            .Returns([new OrderSummaryExitDTO { OrderNumber = 5, ClientId = 2, ClientFullName = "Maria Lopez", Status = "Prepared", TotalCost = 244m, ProductCount = 2 }]);

        var query = new GetOrdersQueryModel { From = from, To = to };
        var result = _controller.GetDispatcherOrders(query) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as List<OrderSummaryResponseModel>;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Count);
        Assert.AreEqual(5, response[0].OrderNumber);
    }

    [TestMethod]
    public void GetOrderById_ValidId_Returns200WithDetail()
    {
        _orderServiceMock
            .Setup(s => s.GetOrderById(10))
            .Returns(new OrderDetailExitDTO
            {
                OrderNumber = 10,
                ClientId = 1,
                ClientFullName = "Juan Garcia",
                OrderDate = DateTime.Today,
                Status = "Pending",
                TotalCost = 183m,
                Products = []
            });

        var result = _controller.GetOrderById(10) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as OrderDetailResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(10, response.OrderNumber);
        Assert.AreEqual("Juan Garcia", response.ClientFullName);
    }

    [TestMethod]
    public void GetOrderById_NotFound_ThrowsKeyNotFoundException()
    {
        _orderServiceMock
            .Setup(s => s.GetOrderById(999))
            .Throws(new KeyNotFoundException("Order 999 not found."));

        Assert.ThrowsException<KeyNotFoundException>(
            () => _controller.GetOrderById(999));
    }

    [TestMethod]
    public void GetClientOrders_ValidRequest_Returns200WithList()
    {
        var expectedOrders = new List<OrderSummaryExitDTO>
        {
            new OrderSummaryExitDTO
            {
                OrderNumber = 1,
                ClientId = 1,
                ClientFullName = "Juan Garcia",
                OrderDate = DateTime.Today,
                Status = "Pending",
                TotalCost = 183m,
                ProductCount = 1
            }
        };

        _orderServiceMock
            .Setup(s => s.GetClientOrders(1, null, null, null))
            .Returns(expectedOrders);

        var result = _controller.GetClientOrders(new GetOrdersQueryModel()) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var response = result.Value as List<OrderSummaryResponseModel>;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Count);
        Assert.AreEqual(1, response[0].OrderNumber);
        Assert.AreEqual("Juan Garcia", response[0].ClientFullName);
    }
}

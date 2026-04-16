using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

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
    public void GetClientOrders_ValidRequest_Returns200WithList()
    {
        var expectedOrders = new List<OrderSummaryDTO>
        {
            new OrderSummaryDTO
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

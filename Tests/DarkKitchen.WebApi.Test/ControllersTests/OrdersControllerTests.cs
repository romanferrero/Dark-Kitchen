using System.Security.Claims;
using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.ControllersTests;

[TestClass]
public class OrdersControllerTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private OrdersController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _orderServiceMock = new Mock<IOrderService>(MockBehavior.Strict);
        _controller = new OrdersController(_orderServiceMock.Object);
    }

    private static CreateOrderRequestModel BuildValidRequest()
    {
        return new CreateOrderRequestModel
        {
            ClientId = 1,
            DeliveryType = "express",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Apartment = "Apto 101",
            Products = ["BURG01", "BURG01"],
        };
    }

    private void SetupUserRole(UserRole role)
    {
        var claims = new List<Claim> { new(ClaimTypes.Role, role.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [TestMethod]
    public void CreateOrder_ValidData_Returns201()
    {
        var expectedResult = new OrderResultDTO
        {
            ClientId = 1,
            OrderNumber = 100,
            Subtotal = 400m,
            ShippingCost = 100m,
            Total = 610m,
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1, "express", "18 de Julio", "1234", "Apto 101",
                It.IsAny<List<string>>()))
            .Returns(expectedResult);

        var result = _controller.CreateOrder(BuildValidRequest());

        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }

    [TestMethod]
    public void CreateOrder_InvalidData_Returns400()
    {
        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
            .Throws(new ArgumentException("Order must have at least one product."));

        Assert.ThrowsException<ArgumentException>(() => _controller.CreateOrder(BuildValidRequest()));
    }

    [TestMethod]
    public void CreateOrder_ValidData_Returns201AndResponseBody()
    {
        var expectedResult = new OrderResultDTO
        {
            ClientId = 1,
            OrderNumber = 100,
            Subtotal = 400m,
            ShippingCost = 100m,
            Total = 610m,
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1, "express", "18 de Julio", "1234", "Apto 101",
                It.IsAny<List<string>>()))
            .Returns(expectedResult);

        var result = _controller.CreateOrder(BuildValidRequest()) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);

        var response = result.Value as CreateOrderResponseModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.ClientId);
        Assert.AreEqual(100, response.OrderNumber);
        Assert.AreEqual(400m, response.Subtotal);
        Assert.AreEqual(100m, response.ShippingCost);
        Assert.AreEqual(610m, response.Total);
    }

    [TestMethod]
    public void CreateOrder_ValidData_CallsServiceWithExpectedArguments()
    {
        var expectedResult = new OrderResultDTO
        {
            ClientId = 1,
            OrderNumber = 100,
            Subtotal = 400m,
            ShippingCost = 100m,
            Total = 610m,
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1, "express", "18 de Julio", "1234", "Apto 101",
                It.Is<List<string>>(items =>
                    items.Count == 2 &&
                    items[0] == "BURG01" &&
                    items[1] == "BURG01")))
            .Returns(expectedResult);

        _controller.CreateOrder(BuildValidRequest());

        _orderServiceMock.Verify(s => s.CreateOrder(
                1, "express", "18 de Julio", "1234", "Apto 101",
                It.Is<List<string>>(items =>
                    items.Count == 2 &&
                    items[0] == "BURG01" &&
                    items[1] == "BURG01")),
            Times.Once);
    }

    [TestMethod]
    public void CreateOrder_InvalidData_Returns400WithMessage()
    {
        const string expectedMessage = "Order must have at least one product.";

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
            .Throws(new ArgumentException(expectedMessage));

        var ex = Assert.ThrowsException<ArgumentException>(() => _controller.CreateOrder(BuildValidRequest()));

        Assert.AreEqual(expectedMessage, ex.Message);
    }

    [TestMethod]
    public void CreateOrder_WithMultipleItems_MapsProductsCorrectly()
    {
        var expectedResult = new OrderResultDTO
        {
            ClientId = 1,
            OrderNumber = 101,
            Subtotal = 900m,
            ShippingCost = 100m,
            Total = 1098m,
        };

        _orderServiceMock
            .Setup(s => s.CreateOrder(
                1, "express", "18 de Julio", "1234", "Apto 101",
                It.Is<List<string>>(items =>
                    items.Count == 3 &&
                    items[0] == "BURG01" &&
                    items[1] == "BURG01" &&
                    items[2] == "PIZZA01")))
            .Returns(expectedResult);

        var request = new CreateOrderRequestModel
        {
            ClientId = 1,
            DeliveryType = "express",
            Street = "18 de Julio",
            DoorNumber = "1234",
            Apartment = "Apto 101",
            Products = ["BURG01", "BURG01", "PIZZA01"],
        };

        var result = _controller.CreateOrder(request);

        Assert.IsInstanceOfType(result, typeof(CreatedResult));

        _orderServiceMock.Verify(s => s.CreateOrder(
                1, "express", "18 de Julio", "1234", "Apto 101",
                It.Is<List<string>>(items =>
                    items.Count == 3 &&
                    items[0] == "BURG01" &&
                    items[1] == "BURG01" &&
                    items[2] == "PIZZA01")),
            Times.Once);
    }

    [TestMethod]
    public void CreateOrder_ProductNotFound_Returns404()
    {
        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
            .Throws(new KeyNotFoundException("Product 'NOEXIST' not found."));

        var ex = Assert.ThrowsException<KeyNotFoundException>(() => _controller.CreateOrder(BuildValidRequest()));

        Assert.AreEqual("Product 'NOEXIST' not found.", ex.Message);
    }

    [TestMethod]
    public void UpdateStatus_ValidData_Returns200()
    {
        var orderId = 1;
        var action = "Prepared";

        SetupUserRole(UserRole.Dispatcher);

        var dto = new UpdateStatusEntryDTO(action);

        var expected = new UpdateStatusExitDTO("Prepared", DateTime.Now);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(orderId, dto))
            .Returns(expected);

        var result = _controller.UpdateStatus(orderId, dto);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(expected, okResult.Value);
    }

    [TestMethod]
    public void UpdateStatus_OrderNotFound_Returns404()
    {
        var orderId = 1;
        var action = "Prepared";

        SetupUserRole(UserRole.Admin);

        var dto = new UpdateStatusEntryDTO(action);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(orderId, dto))
            .Throws(new KeyNotFoundException("Order not found"));

        var result = _controller.UpdateStatus(orderId, dto);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual("Order not found", notFoundResult.Value);
    }

    [TestMethod]
    public void CreateOrder_ServiceThrowsUnexpectedException_Propagates()
    {
        _orderServiceMock
            .Setup(s => s.CreateOrder(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
            .Throws(new InvalidOperationException("Unexpected error"));

        Assert.ThrowsException<InvalidOperationException>(() =>
            _controller.CreateOrder(BuildValidRequest()));
    }

    [TestMethod]
    public void UpdateStatus_NoRoleClaim_ReturnsUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var dto = new UpdateStatusEntryDTO("Prepared");

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void UpdateStatus_Cancel_AdminAllowed_Returns200()
    {
        SetupUserRole(UserRole.Admin);
        var dto = new UpdateStatusEntryDTO("Cancel");
        var expected = new UpdateStatusExitDTO("Cancel", DateTime.Now);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1, dto))
            .Returns(expected);

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateStatus_Cancel_DispatcherNotAllowed_ReturnsUnauthorized()
    {
        SetupUserRole(UserRole.Dispatcher);
        var dto = new UpdateStatusEntryDTO("Cancel");

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void UpdateStatus_OnTheWay_DispatcherAllowed_Returns200()
    {
        SetupUserRole(UserRole.Dispatcher);
        var dto = new UpdateStatusEntryDTO("OnTheWay");
        var expected = new UpdateStatusExitDTO("OnTheWay", DateTime.Now);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1, dto))
            .Returns(expected);

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateStatus_Delivered_DispatcherAllowed_Returns200()
    {
        SetupUserRole(UserRole.Dispatcher);
        var dto = new UpdateStatusEntryDTO("Delivered");
        var expected = new UpdateStatusExitDTO("Delivered", DateTime.Now);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1, dto))
            .Returns(expected);

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateStatus_NotDelivered_DispatcherAllowed_Returns200()
    {
        SetupUserRole(UserRole.Dispatcher);
        var dto = new UpdateStatusEntryDTO("NotDelivered");
        var expected = new UpdateStatusExitDTO("NotDelivered", DateTime.Now);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1, dto))
            .Returns(expected);

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void UpdateStatus_UnknownAction_ReturnsUnauthorized()
    {
        SetupUserRole(UserRole.Admin);
        var dto = new UpdateStatusEntryDTO("AccionDesconocida");

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
    }

    [TestMethod]
    public void UpdateStatus_Prepared_AdminAllowed_Returns200()
    {
        SetupUserRole(UserRole.Admin);
        var dto = new UpdateStatusEntryDTO("Prepared");
        var expected = new UpdateStatusExitDTO("Prepared", DateTime.Now);

        _orderServiceMock
            .Setup(s => s.UpdateStatus(1, dto))
            .Returns(expected);

        var result = _controller.UpdateStatus(1, dto);

        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
    }
}

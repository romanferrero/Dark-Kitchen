using DarkKitchen.Domain;
using DarkKitchen.IBusinessLogic;
using DarkKitchen.WebApi.Controllers;
using DarkKitchen.WebApi.Models;
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

        var request = new UpdateOrderStatusRequestModel
        {
            Status = "Prepared"
        };

        _orderServiceMock
            .Setup(s => s.UpdateStatus(orderId, "Prepared"));

        var result = _controller.UpdateStatus(orderId, request);

        Assert.IsInstanceOfType(result, typeof(OkResult));

        _orderServiceMock.Verify(s => s.UpdateStatus(orderId, "Prepared"), Times.Once);
    }

    [TestMethod]
    public void UpdateStatus_OrderNotFound_ThrowsKeyNotFoundException()
    {
        var orderId = 1;

        var request = new UpdateOrderStatusRequestModel
        {
            Status = "Delivered"
        };

        _orderServiceMock
            .Setup(s => s.UpdateStatus(orderId, "Delivered"))
            .Throws(new KeyNotFoundException("Order not found"));

        var ex = Assert.ThrowsException<KeyNotFoundException>(() =>
            _controller.UpdateStatus(orderId, request));

        Assert.AreEqual("Order not found", ex.Message);
    }
}
